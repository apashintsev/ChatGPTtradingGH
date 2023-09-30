namespace ChatGPTtrading.Domain.Entities;

public class UserAccount : BaseEntity
{
    public Guid UserId { get; private set; }

    /// <summary>
    /// Сколько инвестировано
    /// </summary>
    public decimal InvestedBalance { get; private set; }

    /// <summary>
    /// Накопленная прибыль
    /// </summary>
    public decimal AccumulatedProfit { get; private set; }

    /// <summary>
    /// Сколько было выведено изначальных инвестиций
    /// </summary>
    public decimal WithdrawedInvested { get; private set; }

    public User User { get; private set; }

    public UserAccount()
    {

    }

    public void Deposit(decimal amount)
    {
        InvestedBalance += amount;
    }

    public void WithdrawProfit(decimal amount)
    {
        if (AccumulatedProfit < amount)
        {
            throw new Exception("Недостаточно средств");
        }
        AccumulatedProfit -= amount;
    }

    public void WithdrawInvested(decimal amount)
    {
        if (InvestedBalance < amount)
        {
            throw new Exception("Недостаточно средств");
        }
        InvestedBalance -= amount;
        WithdrawedInvested += amount;
    }

    internal void ChargeProfit(decimal profit)
    {
        AccumulatedProfit += profit;
    }

    internal void Reinvest()
    {
        InvestedBalance += AccumulatedProfit;
        AccumulatedProfit = 0;
    }
}
