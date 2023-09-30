using ChatGPT.Application.Statistics.Dto;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Statistics.Queries;


public record GetActivitiesQuery() : IRequest<IEnumerable<ActivityVm>>;

public class GetActivitiesQueryHandler : IRequestHandler<GetActivitiesQuery, IEnumerable<ActivityVm>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetActivitiesQueryHandler> _logger;

    public GetActivitiesQueryHandler(ApplicationDbContext context, ILogger<GetActivitiesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ActivityVm>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
    {
        var act = await _context.FakeActivities.ToListAsync();
        return act.Select(x => new ActivityVm()
        {
            Id = x.Id,
            Amount = x.Amount,
            CreatedAt = x.CreatedAt,
            Currency = x.Currency,
            Type = x.Type
        });
    }
}
