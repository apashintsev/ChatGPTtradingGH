namespace ChatGPTtrading.Domain.Entities;

public class Invoice : BaseEntity
{
    public int PaymentSystemId { get; private set; }

    public Invoice()
    {
            
    }

    public void SetId(int id)
    {
        PaymentSystemId = id;
    }
}
