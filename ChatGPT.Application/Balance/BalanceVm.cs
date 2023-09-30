namespace ChatGPT.Application.Balance
{
    public class BalanceVm
    {
        public decimal InvestedBalance { get; set; }
        public decimal BlockedInvestedBalance { get; set; }
        public decimal AccumulatedProfit { get; set; }
        public decimal InvestedBalanceCanWithdraw { get; set; }
    }
}
