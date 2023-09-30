using ChatGPTtrading.Domain.Enums;

namespace ChatGPTtrading.Domain.Entities;

public class Withdrawal : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Currency { get; private set; }
    public string WithdrawalMethod { get; private set; }
    public string Address { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime CompletedAt { get; private set; }

    protected Withdrawal()
    {
            
    }

    public Withdrawal(string address, decimal amount, string currency, string withdrawalMethod)
    {
        Address = address;
        Amount = amount;
        Currency = currency;
        WithdrawalMethod = withdrawalMethod;
        Status = TransactionStatus.Waiting;
    }

    public void Reject()
    {
        Status = TransactionStatus.Rejected;
    }

    public void Confirm()
    {
        Status = TransactionStatus.Confirmed;
    }

    public void Error()
    {
        Status = TransactionStatus.Error;
    }

    public void Complete()
    {
        Status = TransactionStatus.Completed;
    }
}
