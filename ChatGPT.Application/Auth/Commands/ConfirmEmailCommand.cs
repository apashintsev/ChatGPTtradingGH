using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using ChatGPTtrading.Infrastructure.Services.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPT.Application.Auth.Commands
{
    public record ConfirmEmailVm(string Email, bool NeedToConfirmEmail);
    public record ConfirmEmailCommand(string Email, string Code) : IRequest<bool>;

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;

        public ConfirmEmailCommandHandler(IAuthService authService, ApplicationDbContext context, ILogger<ConfirmEmailCommandHandler> logger)
        {
            _authService = authService;
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var verifyItem = await _context.VerificationCodes.FirstOrDefaultAsync(x=>x.VerifyingEntity==request.Email);
            if (verifyItem is null)
            {
                //TODO: REAL EXEPTION
                var ex = new Exception(request.Email);
                _logger.LogError(ex.Message);
                throw ex;
            }
            var user = await _authService.FindByEmailAsync(request.Email);

            if (user is null)
            {
                var ex = new UserNotFoundException(request.Email);
                _logger.LogError(ex.Message);
                throw ex;
            }

            //if (user.EmailConfirmed)
            //{
            //    throw new Exception(ExceptionLocalization.GetMessage(Errors.EmailIsVerified, user.Language));
            //}

            if (verifyItem.Attempts > 5)
            {
                //TODO: REAL EXEPTION
                throw new Exception("MaxAttemptsCount");
            }

            if (request.Code != verifyItem.Code)
            {
                verifyItem.AddFailAttmpt();
                await _context.SaveChangesAsync();
                var attempts = 5 - verifyItem.Attempts;
                //TODO: REAL EXEPTION
                throw new Exception("InvalidVerificationCode");
            }

            if (DateTime.UtcNow.Subtract(verifyItem.CreatedAt).TotalMinutes > 60 * 24)
            {
                throw new Exception("VerificationCodeIsExpired");
            }

            await _authService.ConfirmEmailAsync(request.Email);
            _context.VerificationCodes.Remove(verifyItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
