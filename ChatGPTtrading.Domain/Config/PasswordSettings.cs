namespace ChatGPTtrading.Domain.Config;

public class PasswordSettings
{
    public int DefaultLockoutMinutes { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public int RequiredLength { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public bool RequireDigit { get; set; }
}
