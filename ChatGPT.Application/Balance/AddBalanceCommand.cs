using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Balance;


public record AddBalanceCommand(string UserEmail, decimal Amount) : IRequest;

public class AddBalanceCommandHandler : IRequestHandler<AddBalanceCommand>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AddBalanceCommandHandler> _logger;

    public AddBalanceCommandHandler(ApplicationDbContext context, ILogger<AddBalanceCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(AddBalanceCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.Include(x => x.UserAccount).Include(x => x.Activities).FirstOrDefaultAsync(x => x.Email.Trim().ToLower() == request.UserEmail.Trim().ToLower());
        if (user is null)
        {
            var ex = new UserNotFoundException(request.UserEmail);
            _logger.LogError(ex.Message);
            throw ex;
        }
        user.Deposit(request.Amount);
        await _context.SaveChangesAsync();
    }
}
