using Microsoft.AspNetCore.Http;

namespace ChatGPTtrading.Infrastructure.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadFileSystem(IFormFile file);
    }
}