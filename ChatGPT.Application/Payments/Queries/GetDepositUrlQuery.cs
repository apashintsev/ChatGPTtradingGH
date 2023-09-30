using ChatGPTtrading.Domain.Config;
using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace ChatGPT.Application.Payments.Queries;


public record GetDepositUrlQuery(Guid UserId, decimal Amount/*, string PaymentCurrencyCode*/) : IRequest<string>;

public class GetDepositUrlQueryHandler : IRequestHandler<GetDepositUrlQuery, string>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _clientFactory;
    private readonly PaymentServiceConfig _settings;
    private readonly ILogger<GetDepositUrlQueryHandler> _logger;

    public GetDepositUrlQueryHandler(ILogger<GetDepositUrlQueryHandler> logger, IOptions<PaymentServiceConfig> settings, IHttpClientFactory clientFactory, ApplicationDbContext context)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _settings = settings.Value;
        _context = context;
    }

    public async Task<string> Handle(GetDepositUrlQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
                .Include(x => x.UserAccount)
                .Include(x => x.Activities)
                .FirstOrDefaultAsync(x => x.Id == request.UserId);
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

        var invoice = new Invoice();
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var payload = new InvoiceRequest
        {
            TokenApi = _settings.Key,
            Amount = request.Amount,
            Comment = request.UserId.ToString(),
            ReturnUrl = _settings.PaymentPage,
            CallbackUrl = $"{_settings.CallbackUrl}api/payments/CallbackUrl?id={invoice.Id}",
            //TokenCode = request.PaymentCurrencyCode,
            CurrencyCode = "USD"
        };

        var client = _clientFactory.CreateClient();
        var jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{_settings.ApiUrl}/v2/invoice/create", content);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<InvoiceResponse>(jsonResponse);
            invoice.SetId(result.Id);
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Invoice ID: {result.Id}, URL: {result.Url}");
            return result.Url;
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            var errorResult = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse);
            _logger.LogError($"Error: {errorResult.Error}, Messages: {string.Join(", ", errorResult.Message)}");
            return null;
        }
    }
}

public class InvoiceResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
}

public class ErrorResponse
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }
    [JsonProperty("message")]
    public string[] Message { get; set; }
    [JsonProperty("error")]
    public string Error { get; set; }
}

public class InvoiceRequest
{
    [JsonProperty("tokenApi")]
    public string TokenApi { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }

    [JsonProperty("returnUrl")]
    public string ReturnUrl { get; set; }

    [JsonProperty("callbackUrl")]
    public string CallbackUrl { get; set; }

    [JsonProperty("tokenCode")]
    public string TokenCode { get; set; }

    [JsonProperty("currencyCode")]
    public string CurrencyCode { get; set; }
}