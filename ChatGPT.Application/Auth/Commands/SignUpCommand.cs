using ChatGPT.Application.Auth.Dtos;
using ChatGPT.Application.Validators;
using ChatGPTtrading.Domain.Config;
using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using ChatGPTtrading.Infrastructure.Services;
using ChatGPTtrading.Infrastructure.Services.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace ChatGPT.Application.Auth.Commands;

public record SignUpCommand(string Email, string Phone, string Password, string Referral) : IRequest<AuthResultVm>;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, AuthResultVm>
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private ILogger<SignUpCommandHandler> _logger;

    private const string Characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public SignUpCommandHandler(IAuthService authService, ITokenService tokenService, ILogger<SignUpCommandHandler> logger, ApplicationDbContext context)
    {
        _authService = authService;
        _tokenService = tokenService;
        _logger = logger;
        _context = context;
    }

    public async Task<AuthResultVm> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var exsistedEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (exsistedEmail is not null)
        {
            var ex = new UserAllreadyExsistsException(request.Email);
            _logger.LogError(ex.Message);
            throw ex;
        }
        var exsistedPhone = await _context.Users.FirstOrDefaultAsync(x => x.Phone == request.Phone);
        if (exsistedPhone is not null)
        {
            var ex = new UserAllreadyExsistsException(request.Phone);
            _logger.LogError(ex.Message);
            throw ex;
        }
        string refId;
        do
        {
            refId = GenerateRandomString();
        } while (await _context.Users.AsNoTracking().AnyAsync(x => x.ReferralLink == refId));

        var user = new User(request.Email, request.Phone, refId);
        var result = await _authService.CreateUser(user, request.Password);

        if (!string.IsNullOrWhiteSpace(request.Referral) && result)
        {
            var owner = await _context.Users.Include(x => x.Referrals).FirstOrDefaultAsync(x => x.ReferralLink == request.Referral.Trim());
            if (owner is not null)
            {
                var referral = new Referral(owner, user);
                owner.Referrals.Add(referral);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Invalid ref url");
            }
        }

        var token = await _tokenService.CreateToken(user);

        var nickname = $"{user.FirstName} {user.LastName}";

        return new AuthResultVm()
        {
            JwtToken = token,
            Nickname = string.IsNullOrWhiteSpace(nickname) ? user.Email : nickname,
            UserId = user.Id.ToString(),
            AvatarUrl = user.AvatarUrl,
            IsKycPassed = user.KycStatus
        };
    }


    public static string GenerateRandomString(int length = 6)
    {
        StringBuilder result = new(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(Characters[Random.Shared.Next(Characters.Length)]);
        }

        return result.ToString();
    }
}


public class SignUpByEmailCommandValidator : AbstractValidator<SignUpCommand>
{
    private readonly PasswordSettings _passwordSettings;

    public SignUpByEmailCommandValidator(IOptions<PasswordSettings> passwordSettings)
    {
        _passwordSettings = passwordSettings.Value;

        RuleFor(m => m.Email).NotEmpty().WithMessage("Email обязателен");
        RuleFor(m => m.Email).Custom((value, context) =>
        {
            if (!User.IsValidEmail(value))
                context.AddFailure("Некорректный формат Email");
        });

        RuleFor(m => m.Phone).NotEmpty().WithMessage("Телефон обязателен");
        RuleFor(m => m.Phone).Custom((value, context) =>
        {
            if (!User.IsValidPhone(User.CleanPhoneNumber(value)))
                context.AddFailure("Некорректный формат Телефона");
        });

        RuleFor(c => c.Password).PasswordValidatorRules(_passwordSettings);
    }
}
