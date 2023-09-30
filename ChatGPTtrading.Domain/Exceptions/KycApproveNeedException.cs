namespace ChatGPTtrading.Domain.Exceptions;

public class KycApproveNeedException : Exception
{
    public KycApproveNeedException(string email) : base($"Необходимо пройти KYC") { }
}