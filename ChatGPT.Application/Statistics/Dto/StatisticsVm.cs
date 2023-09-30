namespace ChatGPT.Application.Statistics.Dto;

public class StatisticsVm
{
    /// <summary>
    /// Кол-во новых пользователей
    /// </summary>
    public int UsersCount { get; set; }
    /// <summary>
    /// Кол-во новых пользователей успешно прошедших KYC
    /// </summary>
    public int KycApprovedUsersCount { get; set; }
    /// <summary>
    /// Сумма депозитов
    /// </summary>
    public decimal Deposits { get; set; }
    /// <summary>
    /// Сумма накопленных прибылей
    /// </summary>
    public decimal AccumulatedProfit { get; set; }
    /// <summary>
    /// Сумма выплаченных средств
    /// </summary>
    public decimal Withdrawed { get; set; }
    /// <summary>
    /// Реинвестировано
    /// </summary>
    public decimal Reinvested { get; set; }
    /// <summary>
    /// Текущий % прибыли на вклады пользователей
    /// </summary>
    public decimal PayoutPercent { get; set; }

    /// <summary>
    /// Количество приглашенных новых пользователей через реферальные ссылки
    /// </summary>
    public int ReferalsCount { get; set; }

    /// <summary>
    /// общее кол-во депозитов приглашенных пол-лей.
    /// </summary>
    public decimal ReferalDepos { get; set; }
}