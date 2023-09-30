using ChatGPT.Application.Statistics.Dto;
using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Statistics.Commands;


public record AddFakeActivityCommand() : IRequest<int>;

public class AddFakeActivityCommandHandler : IRequestHandler<AddFakeActivityCommand, int>
{
    private readonly ApplicationDbContext _context;
    private readonly IStatisticsNotificationService _statisticsNotificationService;
    private readonly ILogger<AddFakeActivityCommandHandler> _logger;

    public AddFakeActivityCommandHandler(ApplicationDbContext context, ILogger<AddFakeActivityCommandHandler> logger, IStatisticsNotificationService statisticsNotificationService)
    {
        _context = context;
        _logger = logger;
        _statisticsNotificationService = statisticsNotificationService;
    }

    public async Task<int> Handle(AddFakeActivityCommand request, CancellationToken cancellationToken)
    {
        var settings = await _context.PlatformSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            throw new Exception("Settings is null. Impossible");
        }

        var recordsToKeep = _context.FakeActivities
                                      .OrderByDescending(fa => fa.CreatedAt)
                                      .Take(9)
                                      .ToList();

        var recordsToDelete = _context.FakeActivities
                                       .Where(fa => !recordsToKeep.Contains(fa))
                                       .ToList();

        _context.FakeActivities.RemoveRange(recordsToDelete);

        var activity = FakeActivity.CreateRandom(settings.MinFakeActivityValue, settings.MaxFakeActivityValue);
        _context.FakeActivities.Add(activity);


        await _context.SaveChangesAsync();

        await _statisticsNotificationService.ActivityAdded(new ActivityVm()
        {
            Id = activity.Id,
            Amount = activity.Amount,
            CreatedAt = activity.CreatedAt,
            Currency = activity.Currency,
            Type = activity.Type
        });

        return Random.Shared.Next(settings.MinFakeActivityDelayInSeconds, settings.MaxFakeActivityDelayInSeconds);
    }
}
