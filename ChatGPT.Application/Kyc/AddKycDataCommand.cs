using ChatGPT.Application.Kyc;
using ChatGPTtrading.Domain.Entities;
using ChatGPTtrading.Domain.Enums;
using ChatGPTtrading.Domain.Exceptions;
using ChatGPTtrading.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatGPTtrading.API.Controllers
{
    public record AddKycDataCommand(
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string Residency,
        string Citizenship,
        string Occupation,
        string TemporaryResidence,
        string AvatarId,
        string PassportId,
        string PhotoWithPassportId) : IRequest<bool>;

    public class AddKycDataCommandHandler : IRequestHandler<AddKycDataCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ILogger<AddKycDataCommandHandler> _logger;

        public AddKycDataCommandHandler(ApplicationDbContext context, ILogger<AddKycDataCommandHandler> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AddKycDataCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Include(x => x.Documents).FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                var ex = new UserNotFoundException(request.UserId.ToString());
                _logger.LogError(ex.Message);
                throw ex;
            }

            if (user.KycStatus)
            {
                //TODO ALLREADY setted exeption
                var ex = new Exception(request.UserId.ToString());
                _logger.LogError(ex.Message);
                throw ex;
            }

            var avatarUrl = "default";
            if (!string.IsNullOrWhiteSpace(request.AvatarId))
            {
                if (Guid.TryParse(request.AvatarId, out var avatarGuid))
                {
                    var avatar = await _context.Files.FirstOrDefaultAsync(x => x.Id == avatarGuid);
                    if (avatar != null)
                    {
                        avatarUrl = avatar.Url;
                    }
                }
            }

            user.SetKycData(request.FirstName, request.LastName, request.Email, request.Phone,
                request.Residency, request.Citizenship, request.Occupation, request.TemporaryResidence, avatarUrl);
            //TODO EXCEPTIONS WHEN FILES NOT FOUND
            var passportUrl = string.Empty;
            if (Guid.TryParse(request.PassportId, out var passportGuid))
            {
                var passport = await _context.Files.FirstOrDefaultAsync(x => x.Id == passportGuid);
                if (passport != null)
                {
                    user.AddDocument(new Document(DocumentType.Passport, passport.Url));
                    passportUrl = passport.Url;
                }
            }
            var withPassportUrl = string.Empty;
            if (Guid.TryParse(request.PhotoWithPassportId, out var photoWithPpassportGuid))
            {
                var photoWithPpassport = await _context.Files.FirstOrDefaultAsync(x => x.Id == photoWithPpassportGuid);
                if (photoWithPpassport != null)
                {
                    user.AddDocument(new Document(DocumentType.PhotoWithPassport, photoWithPpassport.Url));
                    withPassportUrl = photoWithPpassport.Url;
                }
            }

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            await _mediator.Publish(
                new KycDataSendedEvent(request.UserId, request.FirstName, request.LastName, request.Email, request.Phone,
                                       request.Residency, request.Citizenship, request.Occupation, request.TemporaryResidence,
                                       avatarUrl, passportUrl, withPassportUrl));

            return true;
        }
    }

    public class AddKycDataCommandValidator : AbstractValidator<AddKycDataCommand>
    {
        public AddKycDataCommandValidator()
        {
            RuleFor(m => m.FirstName).NotEmpty();
            RuleFor(m => m.LastName).NotEmpty();
            RuleFor(m => m.Residency).NotEmpty();
            RuleFor(m => m.Citizenship).NotEmpty();
            RuleFor(m => m.Occupation).NotEmpty();
            RuleFor(m => m.TemporaryResidence).NotEmpty();
            RuleFor(m => m.PassportId).NotEmpty().Custom((value, context) =>
            {
                if (!Guid.TryParse(value, out var validGuid))
                    context.AddFailure("Invalid passport id");
            });
            RuleFor(m => m.PhotoWithPassportId).NotEmpty().Custom((value, context) =>
            {
                if (!Guid.TryParse(value, out var validGuid))
                    context.AddFailure("Invalid photo with passport id");
            });
        }
    }
}