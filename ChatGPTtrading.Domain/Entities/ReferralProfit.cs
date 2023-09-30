namespace ChatGPTtrading.Domain.Entities
{
    public class ReferralProfit:BaseEntity
    {
        /// <summary>
        /// связь с основной моделью пользователя
        /// </summary>
        public Guid UserId { get; private set; } 
        /// <summary>
        /// связь с моделью реферала
        /// </summary>
        public Guid ReferralId { get; private set; } 
        /// <summary>
        /// прибыль от реферала
        /// </summary>
        public decimal Profit { get; private set; }
        // <summary>
        // депозит реферала
        // </summary>
        public decimal ReferalDeposit { get; private set; }

        public User User { get;  }
        public Referral Referral { get;  }

        protected ReferralProfit()
        {
                
        }

        public ReferralProfit(Referral referral, decimal profit, decimal referalDeposit)
        {
            Referral = referral;
            Profit = profit;
            ReferalDeposit = referalDeposit;
        }
    }
}
