using ChatGPTtrading.Domain.Entities;

namespace ChatGPTtrading.Infrastructure.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user, string role = "");
    }
}