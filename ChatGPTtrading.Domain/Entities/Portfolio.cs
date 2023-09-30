namespace ChatGPTtrading.Domain.Entities
{
    public class Portfolio:BaseEntity
    {
        public string Currency { get; private set; }
        public string Icon { get; private set; }
        public decimal Percentage { get; private set; }

        public Portfolio(string currency, string icon, decimal percentage)
        {
            Currency = currency;
            Icon = icon;
            Percentage = percentage;
        }
    }
}
