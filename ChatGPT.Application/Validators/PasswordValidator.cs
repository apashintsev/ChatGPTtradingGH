using ChatGPTtrading.Domain.Config;
using FluentValidation;

namespace ChatGPT.Application.Validators
{
    public static class PasswordValidator
    {
        public static IRuleBuilderOptions<T, string> PasswordValidatorRules<T>(this IRuleBuilder<T, string> ruleBuilder, PasswordSettings passwordSettings)
        {
            return ruleBuilder
                    .NotEmpty()
                    .WithMessage("Password Is Empty")
                    .MinimumLength(passwordSettings.RequiredLength)
                    .WithMessage($"Password Length must be not less than {passwordSettings.RequiredLength}")

                    .Matches("[A-Z]").When(x => passwordSettings.RequireUppercase)
                    .WithMessage("Password Requires UppercaseLetter")
                    .Matches("[a-z]").When(x => passwordSettings.RequireLowercase)
                    .WithMessage("Password Requires LowercaseLetter")
                    .Matches("[0-9]").When(x => passwordSettings.RequireDigit)
                    .WithMessage("Password Requires Digits")
                    .Matches("[^a-zA-Z0-9]").When(x => passwordSettings.RequireNonAlphanumeric)
                    .WithMessage("Password Requires Special Character");
        }
    }
}
