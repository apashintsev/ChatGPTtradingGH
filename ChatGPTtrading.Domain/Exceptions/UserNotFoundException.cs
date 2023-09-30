namespace ChatGPTtrading.Domain.Exceptions
{
    public class UserNotFoundException:Exception
    {
        public UserNotFoundException(string email):base($"Пользователь '{email}' не найден."){}
    }
}
