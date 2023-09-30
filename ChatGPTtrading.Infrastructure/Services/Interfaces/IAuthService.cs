using ChatGPTtrading.Domain.Entities;

namespace ChatGPTtrading.Infrastructure.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> CheckPasswordSignInAsync(User user, string password);
        Task ConfirmEmailAsync(string email);
        Task<bool> CreateUser(User user, string password);
        Task<User> FindByEmailAsync(string email);
    }
}