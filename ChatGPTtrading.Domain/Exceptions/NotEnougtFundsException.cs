namespace ChatGPTtrading.Domain.Exceptions
{
    public class NotEnougtFundsException : Exception
    {
        public NotEnougtFundsException(string accountType) : base($"Недостаточно денег на счету {accountType}") { }
    }
}
