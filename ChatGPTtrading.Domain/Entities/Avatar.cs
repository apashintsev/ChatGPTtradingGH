namespace ChatGPTtrading.Domain.Entities
{
    public class FileItem:BaseEntity
    {
        public string Url { get; private set; }

        protected FileItem() { }

        public FileItem(string url)
        {
            Url = url;
        }
    }
}
