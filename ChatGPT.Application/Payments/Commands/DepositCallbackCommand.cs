using ChatGPTtrading.Domain.Config;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace ChatGPT.Application.Payments.Commands;


public record DepositCallbackCommand(Guid MyId) : IRequest<decimal>;

public class DepositCallbackCommandHandler : IRequestHandler<DepositCallbackCommand, decimal>
{
    private readonly ILogger<DepositCallbackCommandHandler> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly PaymentServiceConfig _settings;
    private readonly ApplicationDbContext _context;

    public DepositCallbackCommandHandler(ILogger<DepositCallbackCommandHandler> logger, ApplicationDbContext context, IHttpClientFactory clientFactory, IOptions<PaymentServiceConfig> settings)
    {
        _logger = logger;
        _context = context;
        _clientFactory = clientFactory;
        _settings = settings.Value;
    }

    public async Task<decimal> Handle(DepositCallbackCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices.FirstOrDefaultAsync(x => x.Id == request.MyId);
        if (invoice == null)
        {
            throw new Exception("Что-то не так с платежкой");
        }

        var payload = new InfoRequest
        {
            TokenApi = _settings.Key,
            Id = invoice.PaymentSystemId
        };

        var client = _clientFactory.CreateClient();
        var jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_settings.ApiUrl}/v2/invoice/info", content);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<InfoResponse>(jsonResponse);

            if (result.Status == "SUCCESS")
            {
                var userId = Guid.Parse(result.Comment);
                var user = await _context.Users
                    .Include(x => x.UserAccount)
                    .Include(x => x.Activities)
                    .FirstOrDefaultAsync(x => x.Id == userId);
                if (user is null)
                {
                    _logger.LogWarning("User is null");
                    throw new Exception();
                }
                if (!user.KycStatus && !user.KycSended)
                {
                    throw new Exception("Перед пополнением пожалуйста пройдите процедуру KYC.");
                }
                if (!user.KycStatus && user.KycSended)
                {
                    throw new Exception("Пожалуйста дождитесь одобрения заявки на KYC. Среднее время ожидания 15-30 минут");
                }

                user.Deposit(result.Amount);
                var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
                //теперь надо начислить рефералу, если позволяет депозит
                if (user.UserAccount.InvestedBalance >= settings.RefferalTreshold && !user.IsReferralThresholdPayed)
                {
                    //найдём того кто привёл
                    var referer = await _context.Users.Include(x => x.UserAccount)
                    .Include(x => x.Activities)
                    .Include(x => x.Referrals)
                    .FirstOrDefaultAsync(x => x.Referrals.Select(x => x.ReferralUserId).Contains(userId));
                    var refUser = await _context.Referrals.FirstOrDefaultAsync(x => x.ReferralUser.Id == referer.Id);
                    referer.AddOneTimeRefPayout(refUser, settings.RefferalPayout, user.UserAccount.InvestedBalance);
                    user.RefTresholdPayoutDone();
                }

                await _context.SaveChangesAsync();
                return user.GetInvestedBalance();
            }
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse);
            _logger.LogError($"Error: {errorResult.Error}, Messages: {errorResult.Message}");

        }
        return 0l;
    }
}

public class InfoRequest
{
    [JsonProperty("tokenApi")]
    public string TokenApi { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }
}

public class InfoResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }

    [JsonProperty("returnUrl")]
    public string ReturnUrl { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("createAt")]
    public string CreateAt { get; set; }
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