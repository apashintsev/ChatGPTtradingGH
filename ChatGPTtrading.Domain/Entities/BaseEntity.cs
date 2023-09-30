namespace ChatGPTtrading.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
