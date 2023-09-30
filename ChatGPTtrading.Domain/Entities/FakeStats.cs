using System.Security.Cryptography;

namespace ChatGPTtrading.Domain.Entities;

public class FakeStats : BaseEntity
{
    public int TotalInvestorsCount { get; private set; }
    public int TodayInvestorsCount { get; private set; }
    public int NeededInvestorsCount { get; private set; }


    public decimal TotalWithdrawalsAmount { get; private set; }
    public decimal TodayWithdrawalsAmount { get; private set; }
    public decimal NeededWithdrawalsAmount { get; private set; }

    public decimal MinWithdrawalAmount { get; private set; }
    public decimal MaxWithdrawalAmount { get; private set; }

    public DateTime TodayDatetime { get; private set; }

    protected FakeStats()
    {
        TodayDatetime = DateTime.UtcNow;
    }

    public FakeStats(int totalInvestors, int investorsPerDay, decimal totalWithdrawals, decimal withdrawalsPerDay, decimal min, decimal max)
    {
        TotalInvestorsCount = totalInvestors;
        TotalWithdrawalsAmount = totalWithdrawals;
        NeededInvestorsCount = investorsPerDay;
        NeededWithdrawalsAmount = withdrawalsPerDay;
        MinWithdrawalAmount = min;
        MaxWithdrawalAmount = max;
        TodayDatetime = DateTime.UtcNow;
    }

    public TimeSpan GetNextInvestorInterval()
    {
        int remainingInvestors = NeededInvestorsCount - TodayInvestorsCount;
        TimeSpan remainingTime = new TimeSpan(24, 0, 0) - (DateTime.UtcNow - TodayDatetime);
        if (remainingInvestors <= 0 || remainingTime <= TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        // Вычисление базового интервала в миллисекундах
        var remainingTimeMs = (int)remainingTime.TotalMilliseconds;
        var baseIntervalMs = remainingTimeMs / remainingInvestors;

        // Добавляем "шум" к интервалу. Например, в пределах ±10% от базового интервала
        var noise = (baseIntervalMs * 0.30) * (Random.Shared.NextDouble() * 2 - 1); // Возвращает значение между -1 и 1

        var nextIntervalMs = baseIntervalMs + (int)noise;

        return TimeSpan.FromMilliseconds(nextIntervalMs);
    }


    public void AddInvestor()
    {
        if (DateTime.UtcNow.Date == TodayDatetime.Date)
        {
            TodayInvestorsCount += 1;
        }
        else
        {
            TodayDatetime = DateTime.UtcNow;
            TodayInvestorsCount = 1;
        }
        if (TodayInvestorsCount < NeededInvestorsCount)
        {
            TotalInvestorsCount += 1;
        }
    }

    public TimeSpan GetNextWithdrawInterval()
    {
        decimal remainingWithdrawals = NeededWithdrawalsAmount - TodayWithdrawalsAmount;
        TimeSpan remainingTime = new TimeSpan(24, 0, 0) - (DateTime.UtcNow - TodayDatetime);

        if (remainingWithdrawals <= 0 || remainingTime <= TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        int remainingIntervals = (int)Math.Ceiling(remainingWithdrawals / MinWithdrawalAmount);

        // Вычисление базового интервала в миллисекундах
        var remainingTimeMs = (int)remainingTime.TotalMilliseconds;
        var baseIntervalMs = remainingTimeMs / remainingIntervals;

        // Добавляем "шум" к интервалу. Например, в пределах ±10% от базового интервала
        var noise = (baseIntervalMs * 0.30) * (Random.Shared.NextDouble() * 2 - 1); // Возвращает значение между -1 и 1

        var nextIntervalMs = baseIntervalMs + (int)noise;

        return TimeSpan.FromMilliseconds(nextIntervalMs);
    }


    public void AddWithdraw()
    {
        // Оставшаяся сумма выводов
        decimal remainingWithdrawals = NeededWithdrawalsAmount - TodayWithdrawalsAmount;

        // Если нечего выводить, выходим
        if (remainingWithdrawals <= 0)
        {
            return;
        }

        // Определяем минимальную и максимальную сумму для текущего вывода
        decimal minAmount = Math.Min(MinWithdrawalAmount, remainingWithdrawals);
        decimal maxAmount = Math.Min(MaxWithdrawalAmount, remainingWithdrawals);


        // Случайная сумма вывода
        decimal amount = GenerateNumber((int)minAmount, (int)maxAmount + 1);

        // Добавляем сумму вывода к сегодняшним и общим суммам выводов
        if (DateTime.UtcNow.Date == TodayDatetime.Date)
        {
            TodayWithdrawalsAmount += amount;
        }
        else
        {
            TodayDatetime = DateTime.UtcNow;
            TodayWithdrawalsAmount = amount;
        }

        if (TodayWithdrawalsAmount < NeededWithdrawalsAmount)
        {
            TotalWithdrawalsAmount += amount;
        }
    }


    public void SetInvestorsSettings(int needed)
    {
        NeededInvestorsCount = needed;
    }
    public void SetWithdrawalsSettings(decimal needed, decimal min, decimal max)
    {
        NeededWithdrawalsAmount = needed;
        MinWithdrawalAmount = min;
        MaxWithdrawalAmount = max;
    }


    private static decimal GenerateNumber(decimal min, decimal max)
    {
        if (min > max)
        {
            throw new ArgumentOutOfRangeException("Min cannot be greater than Max.");
        }

        using var rng = RandomNumberGenerator.Create();
        byte[] data = new byte[4];
        rng.GetBytes(data);

        var generatedNumber = BitConverter.ToUInt32(data, 0);

        var diff = max - min;
        var mod = (int)(generatedNumber % (diff + 1));
        return min + mod;
    }
}
