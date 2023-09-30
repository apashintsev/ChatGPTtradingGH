using FluentValidation.Results;

namespace ChatGPTtrading.Domain.Exceptions
{
    public class RuValidationException:Exception
    {
        public RuValidationException(IEnumerable<ValidationFailure> errors): base(BuildErrorMessage(errors))
        {
        }

        private static string BuildErrorMessage(IEnumerable<ValidationFailure> errors)
        {
            var arr = errors.Select(x => $"{Environment.NewLine} {x.ErrorMessage}");
            return "Ошибка валидации: " + string.Join(string.Empty, arr);
        }
    }
}
