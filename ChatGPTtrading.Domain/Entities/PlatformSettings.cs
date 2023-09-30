namespace ChatGPTtrading.Domain.Entities;

public class PlatformSettings : BaseEntity
{
    /// <summary>
    /// Текущая прибыль % месяц, начисляется раз в день
    /// </summary>
    public decimal ProfitRate { get; private set; }

    /// <summary>
    /// Текущий % прибыли от приглашенного реферала
    /// </summary>
    public decimal ReferralRate { get; private set; }

    /// <summary>
    /// сумма депозитов в долларах после которой начисляется бонус за приведенного пользователя
    /// </summary>
    public decimal RefferalTreshold { get; private set; }

    /// <summary>
    /// фиксированная плата за реферала с депозитом больше чем referral _threshold
    /// </summary>
    public decimal RefferalPayout { get; private set; }

    /// <summary>
    /// Период заморозки
    /// </summary>
    public int HoldPeriodInMinutes { get; private set; }

    /// <summary>
    /// Минимальная сумма в фейковой активности
    /// </summary>
    public decimal MinFakeActivityValue { get; private set; }

    /// <summary>
    /// Максимальная сумма в фейковой активности
    /// </summary>
    public decimal MaxFakeActivityValue { get; private set; }

    /// <summary>
    /// Минимальная задержка перед добавлением новой фейковой активности
    /// </summary>
    public int MinFakeActivityDelayInSeconds { get; private set; }

    /// <summary>
    /// Максимальная задержка перед добавлением новой фейковой активности
    /// </summary>
    public int MaxFakeActivityDelayInSeconds { get; private set; }


    protected PlatformSettings()
    {

    }

    public PlatformSettings(decimal profitRate, decimal referralRate, decimal refferalTreshold, decimal refferalPayout, int holdPeriodInMinutes, int minFakeActivityDelayInSeconds, int maxFakeActivityDelayInSeconds, decimal minFakeActivityValue, decimal maxFakeActivityValue = 0)
    {
        ProfitRate = profitRate;
        ReferralRate = referralRate;
        RefferalTreshold = refferalTreshold;
        RefferalPayout = refferalPayout;
        HoldPeriodInMinutes = holdPeriodInMinutes;
        MinFakeActivityDelayInSeconds = minFakeActivityDelayInSeconds;
        MaxFakeActivityDelayInSeconds = maxFakeActivityDelayInSeconds;
        MinFakeActivityValue = minFakeActivityValue;
        MaxFakeActivityValue = maxFakeActivityValue;
    }

    public void SetProfitRate(decimal v)
    {
        if (v < 0) throw new Exception("Процент не может быть отрицательным");
        ProfitRate = v;
    }

    public void SetReferralRate(decimal v)
    {
        if (v < 0) throw new Exception("Процент не может быть отрицательным");
        ReferralRate = v;
    }

    public void SetReferralTreshold(decimal v)
    {
        if (v < 0) throw new Exception("Сумма не может быть отрицательной");
        RefferalTreshold = v;
    }

    public void SetRefferalPayout(decimal v)
    {
        if (v < 0) throw new Exception("Сумма не может быть отрицательной");
        RefferalPayout = v;
    }

    public void SetHoldPeriodInMinutes(int v)
    {
        if (v < 0) throw new Exception("Время в минутах не может быть отрицательным");
        HoldPeriodInMinutes = v;
    }

    public void SetMinFakeActivityValue(decimal v)
    {
        if (v < 0) throw new Exception("Сумма не может быть отрицательной");
        if (v >= MaxFakeActivityValue) throw new Exception("Сумма не может быть больше максимальной");
        MinFakeActivityValue = v;
    }
    public void SetMaxFakeActivityValue(decimal v)
    {
        if (v < 0) throw new Exception("Сумма не может быть отрицательной");
        if (v <= MinFakeActivityValue) throw new Exception("Сумма не может быть меньше минимальной");
        MaxFakeActivityValue = v;
    }

    public void SetMinFakeActivityDelayInSeconds(int v)
    {
        if (v < 0) throw new Exception("Время в минутах не может быть отрицательным");
        if (v >= MaxFakeActivityDelayInSeconds) throw new Exception("Время в минутах не может быть больше макс");
        MinFakeActivityDelayInSeconds = v;
    }

    public void SetMaxFakeActivityDelayInSeconds(int v)
    {
        if (v < 0) throw new Exception("Время в минутах не может быть отрицательным");
        if (v <= MinFakeActivityDelayInSeconds) throw new Exception("Время в минутах не может быть меньше мин");
        MaxFakeActivityDelayInSeconds = v;
    }
}
