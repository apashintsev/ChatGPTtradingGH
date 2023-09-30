using ChatGPTtrading.Domain.Enums;

namespace ChatGPTtrading.Domain.Entities;

public class Activity:BaseEntity
{
    public Guid UserId { get; private set; }

    public ActionType ActionType { get; private set; }

    public string Currency { get; private set; }

    public decimal Amount { get; private set; }

    protected Activity()
    {
            
    }

    public Activity(ActionType actionType, string currency, decimal amount)
    {
        ActionType = actionType;
        Currency = currency;
        Amount = amount;
    }
}
