using ChatGPTtrading.Domain.Enums;

namespace ChatGPTtrading.Domain.Entities
{
    public class Document : BaseEntity
    {
        public Guid UserId { get; private set; }
        public DocumentType Type { get; private set; }
        public DateTime UploadDate { get; private set; }

        public string FileUrl { get; set; }

        public User User { get; private set; }


        protected Document() { }

        public Document(DocumentType type, string fileUrl)
        {
            Type = type;
            UploadDate = DateTime.UtcNow;
            FileUrl = fileUrl;
        }
    }
}
