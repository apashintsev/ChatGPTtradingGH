using ChatGPT.Application.Payments.Dtos;
using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Payments.Queries;


public record GetWithdrawalsQuery(Guid UserId) : IRequest<IList<WithdrawalVm>>;

public class GetWithdrawalsQueryHandler : IRequestHandler<GetWithdrawalsQuery, IList<WithdrawalVm>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetWithdrawalsQueryHandler> _logger;

    public GetWithdrawalsQueryHandler(ApplicationDbContext context, ILogger<GetWithdrawalsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IList<WithdrawalVm>> Handle(GetWithdrawalsQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.Include(x => x.Withdrawals).FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null)
        {
            var ex = new UserNotFoundException(request.UserId.ToString());
            _logger.LogError(ex.Message);
            throw ex;
        }
        return user.Withdrawals.Select(x => new WithdrawalVm()
        {
            Address = x.Address,
            Amount = x.Amount,
            PaymentMethod = x.WithdrawalMethod,
            Date = x.CreatedAt.ToString("dd.MM.yyyy"),
            Status = x.Status
        }).ToArray();
    }
}
