using ChatGPTtrading.Domain.Config;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace ChatGPT.Application.Payments.Events;

public record WithdrawApprovedEvent(
    Guid TransactionId
) : INotification;

public class WithdrawApprovedEventHandler : INotificationHandler<WithdrawApprovedEvent>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<WithdrawApprovedEventHandler> _logger;
    private readonly PaymentServiceConfig _settings;

    public WithdrawApprovedEventHandler(ILogger<WithdrawApprovedEventHandler> logger, ApplicationDbContext context, IHttpClientFactory clientFactory, IOptions<PaymentServiceConfig> settings)
    {
        _logger = logger;
        _context = context;
        _clientFactory = clientFactory;
        _settings = settings.Value;
    }

    public async Task Handle(WithdrawApprovedEvent notification, CancellationToken cancellationToken)
    {
        var transaction = await _context.Withdrawals.FirstOrDefaultAsync(x => x.Id == notification.TransactionId);
        if (transaction == null)
        {
            //TODO real exception
            throw new Exception("Transaction not found");
        }

        try
        {
            var payload = new BalanceRequest
            {
                TokenApi = _settings.Key
            };

            var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_settings.ApiUrl}/v2/token/getAllBalance", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BalanceResponse>(jsonResponse);
                var tokenBalance = transaction.WithdrawalMethod switch
                {
                    "USDTTRC20" => result.USDTTRC20,
                    "USDTBEP20" => result.USDTBEP20,
                    _ => result.USDTTRC20,
                };

            
                if (result.USDTERC20 >= transaction.Amount)
                {
                    var payloadSend = new SendMoneyRequest
                    {
                        TokenApi = _settings.Key,
                        AddressTo = transaction.Address,
                        IncludeFee = false,
                        Amount = transaction.Amount,
                        Token = transaction.WithdrawalMethod
                    };
                    var contentSend = new StringContent(JsonConvert.SerializeObject(payloadSend), Encoding.UTF8, "application/json");

                    var responseSend = await client.PostAsync($"{_settings.ApiUrl}/v2/token/sendTransaction", content);

                    if (responseSend.IsSuccessStatusCode)
                    {
                        transaction.Complete();
                    }
                }
                else
                {
                    //TODO msg to tg
                    transaction.Error();
                }
            }
            else
            {
                transaction.Error();
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse);
                _logger.LogError($"Error: {errorResult.Error}, Messages: {errorResult.Message}");
                //TODO msg to tg
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Transaction {notification.TransactionId} withdraw error!");
            transaction.Error();
        }

        await _context.SaveChangesAsync();
    }
}

public class BalanceRequest
{
    [JsonProperty("tokenApi")]
    public string TokenApi { get; set; }
}

public class BalanceResponse
{
    [JsonProperty("BTC")]
    public decimal BTC { get; set; }

    [JsonProperty("TRX")]
    public decimal TRX { get; set; }

    [JsonProperty("LTC")]
    public decimal LTC { get; set; }

    [JsonProperty("XRP")]
    public decimal XRP { get; set; }

    [JsonProperty("BNB")]
    public decimal BNB { get; set; }

    [JsonProperty("ETH")]
    public decimal ETH { get; set; }

    [JsonProperty("SOL")]
    public decimal SOL { get; set; }

    [JsonProperty("USDTTRC20")]
    public decimal USDTTRC20 { get; set; }

    [JsonProperty("USDTERC20")]
    public decimal USDTERC20 { get; set; }  
    [JsonProperty("USDTBEP20")]
    public decimal USDTBEP20 { get; set; }
}

public class ErrorResponse
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
    [JsonProperty("error")]
    public string Error { get; set; }
}

public class SendMoneyRequest
{
    [JsonProperty("tokenApi")]
    public string TokenApi { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("addressTo")]
    public string AddressTo { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("includeFee")]
    public bool IncludeFee { get; set; }
}