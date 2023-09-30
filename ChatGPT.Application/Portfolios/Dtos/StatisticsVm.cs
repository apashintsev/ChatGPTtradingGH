namespace ChatGPT.Application.Portfolios.Dtos
{
    public class StatisticsVm
    {
        public decimal InvestmentBalance { get; set; }
        public decimal EarningsLast30Days { get; set; }
        public decimal TotalEarnings { get; set; }
        public List<string> Dates { get; set; }
        public List<DiagramDataVm> Data { get; set; }
    }

    public class DiagramDataVm
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
    }

}
