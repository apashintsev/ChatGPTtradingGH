namespace ChatGPTtrading.Models
{
    public class BaseResponse<T>
    {
        public T Data { get; set; }
        public List<string> Errors { get; set; }

        public BaseResponse(T data)
        {
            Data = data;
        }

        public void AddError(string message)
        {
            if (Errors is null)
            {
                Errors = new List<string>();
            }

            Errors.Add(message);
        }
    }
}
