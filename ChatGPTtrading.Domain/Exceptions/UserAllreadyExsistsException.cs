namespace ChatGPTtrading.Domain.Exceptions
{
    public class UserAllreadyExsistsException : Exception
    {
        public UserAllreadyExsistsException(string value)
            : base($"Пользователь с таким {(value.Contains('@') ? "email" : "телефоном")} [{value}] уже существует.")
        {

        }
    }
}
