using ChatGPT.Application.Statistics.Dto;

namespace ChatGPT.Application.Statistics;

public interface IStatisticsNotificationService
{
    public Task StatisticsUpdated(StatTotalVm stat);

    public Task ActivityAdded(ActivityVm activity);
}
