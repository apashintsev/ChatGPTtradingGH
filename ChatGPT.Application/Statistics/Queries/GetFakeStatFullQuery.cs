using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Statistics.Queries;


public record GetFakeStatFullQuery() : IRequest<FakeStats>;

public class GetFakeStatFullQueryHandler : IRequestHandler<GetFakeStatFullQuery, FakeStats>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GetFakeStatFullQueryHandler> _logger;

    public GetFakeStatFullQueryHandler(ApplicationDbContext context, ILogger<GetFakeStatFullQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FakeStats> Handle(GetFakeStatFullQuery request, CancellationToken cancellationToken)
    {
        var stat = await _context.FakeStats.FirstOrDefaultAsync();
        if (stat is null)
        {
            stat = new FakeStats(12_000, 3500, 120_000, 20_000, 10, 100);
        }

        return stat;
    }
}
