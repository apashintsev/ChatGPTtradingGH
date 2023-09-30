namespace ChatGPTtrading.Domain.Entities;

public class FakeActivity
{
    public int Id { get; private set; }
    public ActivityType Type { get; private set; }
    public string Currency { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected FakeActivity()
    {
            
    }

    public static FakeActivity CreateRandom(decimal minAmount, decimal maxAmount)
    {
        var fa = new FakeActivity();
        var types = Enum.GetValues(typeof(ActivityType));
        fa.Type = (ActivityType)types!.GetValue(Random.Shared.Next(types!.Length));
        string[] currencies = { "BTC", "USDT", "ETH" };
        fa.Currency = currencies[Random.Shared.Next(currencies.Length)];
        fa.Amount = (decimal)(Random.Shared.NextDouble() * (double)(maxAmount - minAmount) + (double)minAmount);
        fa.CreatedAt = DateTime.UtcNow;

        return fa;
    }
}

public enum ActivityType
{
    NewUser,
    Deposit
}