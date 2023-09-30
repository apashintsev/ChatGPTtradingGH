namespace ChatGPTtrading.Domain.Exceptions;

public class IncorrectPasswordException : Exception
{
    public IncorrectPasswordException(string email) : base($"Неправильный пароль для '{email}'") { }
}
