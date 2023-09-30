namespace ChatGPTtrading.Domain.Enums
{
    public enum ActionType
    {
        /// <summary>
        /// Депозит
        /// </summary>
        Deposit,
        /// <summary>
        /// Начисление профита
        /// </summary>
        ChargeProfit,
        /// <summary>
        /// Выплата за реферала
        /// </summary>
        ReferalPayout,
        /// <summary>
        /// Разовая выплата за реферала
        /// </summary>
        OneTimeReferalPayout,
        /// <summary>
        /// Вывод прибыли
        /// </summary>
        WithdrawProfit,
        /// <summary>
        /// Вывод инвестированного
        /// </summary>
        WithdrawInvested,
        /// <summary>
        /// Реинвест
        /// </summary>
        Reinvest
    }
}
