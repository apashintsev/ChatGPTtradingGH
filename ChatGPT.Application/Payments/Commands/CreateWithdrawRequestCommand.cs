using ChatGPT.Application.Payments.Dtos;
using ChatGPT.Application.Payments.Events;
using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ChatGPT.Application.Payments.Commands;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountType
{
    Profit,
    Invested
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WithdrawNetwork
{
    USDTBEP20, USDTTRC20
}


public record CreateWithdrawRequestCommand(Guid UserId, AccountType Type, decimal Amount, string Address, WithdrawNetwork Network) : IRequest<WithdrawalVm>;

public class CreateWithdrawRequestCommandHandler : IRequestHandler<CreateWithdrawRequestCommand, WithdrawalVm>
{
    private readonly ApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateWithdrawRequestCommandHandler> _logger;

    public CreateWithdrawRequestCommandHandler(ILogger<CreateWithdrawRequestCommandHandler> logger, ApplicationDbContext context, IMediator mediator)
    {
        _logger = logger;
        _context = context;
        _mediator = mediator;
    }

    public async Task<WithdrawalVm> Handle(CreateWithdrawRequestCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.Include(x => x.UserAccount).Include(x => x.Withdrawals).Include(x => x.Activities).FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null)
        {
            var ex = new UserNotFoundException(request.UserId.ToString());
            _logger.LogError(ex.Message);
            throw ex;
        }

        if (!user.KycStatus)
        {
            var ex = new KycApproveNeedException(request.UserId.ToString());
            _logger.LogError(ex.Message);
            throw ex;
        }

        if (request.Type == AccountType.Profit && user.GetProfitBalance() < request.Amount)
        {
            var ex = new NotEnougtFundsException("Профит");
            _logger.LogError(ex.Message);
            throw ex;
        }
        var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
        if (request.Type == AccountType.Invested && user.GetInvestedBalanceThatUserCanWithdraw(settings.HoldPeriodInMinutes) < request.Amount)
        {
            var ex = new NotEnougtFundsException("Инвест");
            _logger.LogError(ex.Message);
            throw ex;
        }

        user.AddWithdrawal(request.Address, request.Amount, "USDT", request.Network.ToString());

        await _context.SaveChangesAsync();

        var last = user.Withdrawals.Last();

        await _mediator.Publish(new WithdrawRequestedEvent(request.UserId, last.Id, user.Email,
            user.Phone, user.FirstName, user.LastName, request.Amount, last.Currency, last.CreatedAt));

        return new WithdrawalVm()
        {
            Address = last.Address,
            Amount = last.Amount,
            PaymentMethod = last.WithdrawalMethod,
            Date = last.CreatedAt.ToString("dd.MM.yyyy"),
            Status = last.Status
        };
    }
}

public class CreateWithdrawRequestCommandValidator : AbstractValidator<CreateWithdrawRequestCommand>
{
    public CreateWithdrawRequestCommandValidator()
    {
        RuleFor(m => m.Amount).NotEmpty().WithMessage("Нельзя вывести 0");
        RuleFor(m => m.Address)
            .NotEmpty().WithMessage("Не указан адрес для вывода")
            .Must(BeValidEthereumAddress).WithMessage("Неверный формат адреса");
    }
    private bool BeValidEthereumAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return false;

        // Проверяем, что адрес начинается с "0x" и имеет длину 42 символа.
        // Также проверяем, что адрес содержит только 16-ричные символы.
        var ethereumAddressPattern = @"^0x[a-fA-F0-9]{40}$";
        return Regex.IsMatch(address, ethereumAddressPattern);
    }
}
