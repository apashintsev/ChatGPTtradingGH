namespace ChatGPT.Application.Referals.Dtos
{
    public class RefRewardsVm
    {
        public decimal TotalEarned { get; set; }
        public List<ReferalDataVm> List { get; set; }

    }

    public class ReferalDataVm
    {
        /// <summary>
        /// Реферал
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Объем депозитов
        /// </summary>
        public decimal Deposit { get; set; }

        /// <summary>
        /// Прибыль
        /// </summary>
        public decimal Earned { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }
    }
}
