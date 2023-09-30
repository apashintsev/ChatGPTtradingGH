using Amazon.S3.Model;
using ChatGPT.Application.Statistics;
using ChatGPT.Application.Statistics.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ChatGPTtrading.API.SignalR;

[AllowAnonymous]
public class StatisticsHub : Hub<IStatisticsNotificationService>, IStatisticsNotificationService
{
    private readonly ILogger<StatisticsHub> _logger;
    private readonly IHubContext<StatisticsHub, IStatisticsNotificationService> _context;

    public StatisticsHub(ILogger<StatisticsHub> logger, IHubContext<StatisticsHub, IStatisticsNotificationService> context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task ActivityAdded(ActivityVm activity)
    {
        await _context.Clients.All.ActivityAdded(activity);
        _logger.LogInformation($"Hub Statistics updated: {JsonConvert.SerializeObject(activity)}");
    }

    public async Task StatisticsUpdated(StatTotalVm stat)
    {
        await _context.Clients.All.StatisticsUpdated(stat);
        _logger.LogInformation($"Hub Statistics updated: {JsonConvert.SerializeObject(stat)}");
    }
}
