using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTtrading.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly ApplicationDbContext _context;
    public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<bool> CheckPasswordSignInAsync(User user, string password)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser is not null)
        {
            return await _userManager.CheckPasswordAsync(appUser, password);
        }
        return false;
    }

    public async Task ConfirmEmailAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        await _userManager.UpdateAsync(appUser);
    }

    public async Task<bool> CreateUser(User user, string password)
    {
        try
        {
            // Adding the domain user entity
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var appUser = new ApplicationUser(user.Id)
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.Phone,
                UserName = user.Email
            };

            var result = await _userManager.CreateAsync(appUser, password);
            if (!result.Succeeded)
            {
                //TODO: DETAIL REASON, cant ctreate user ex, see errors
                throw new Exception();
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public async Task<User> FindByEmailAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            return null;
        }
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == appUser.Id);
    }

}
