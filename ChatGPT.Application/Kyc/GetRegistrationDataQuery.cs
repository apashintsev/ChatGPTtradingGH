using ChatGPT.Application.Kyc.Dto;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Kyc;

public record GetRegistrationDataQuery(Guid UserId) : IRequest<RegistrationDataVm>;

public class GetRegistrationDataQueryHandler : IRequestHandler<GetRegistrationDataQuery, RegistrationDataVm>
{
    private readonly ILogger<GetRegistrationDataQueryHandler> _logger;
    private readonly ApplicationDbContext _context;

    public GetRegistrationDataQueryHandler(ILogger<GetRegistrationDataQueryHandler> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<RegistrationDataVm> Handle(GetRegistrationDataQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user is null)
        {
            //TODO set exeption
            throw new Exception();
        }
        return new RegistrationDataVm()
        {
            Email = user.Email,
            Phone = user.Phone
        };
    }
}
