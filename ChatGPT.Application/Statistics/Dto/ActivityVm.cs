using ChatGPTtrading.Domain.Entities;

namespace ChatGPT.Application.Statistics.Dto;

public class ActivityVm
{
    public int Id { get; set; }
    public ActivityType Type { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
