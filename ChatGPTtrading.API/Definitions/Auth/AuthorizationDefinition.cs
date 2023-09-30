using ChatGPTtrading.API.Definitions.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace ChatGPTtrading.API.Definitions.Auth;

/// <summary>
/// Authorization Policy registration
/// </summary>
public class AuthorizationDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        // Get secret string from appsettings and add authentication
        var secretKey = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Secret")!);
        var issuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer");
        var audience  = builder.Configuration.GetValue<string>("JwtSettings:Audience");

        var tokenValidationParameters =  new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            RequireAudience = true,
            ValidateAudience = true,
            ValidAudience = audience,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        services.AddSingleton(tokenValidationParameters);

        builder.Services.AddAuthentication().AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.Audience = audience;
            options.ClaimsIssuer = issuer;
            options.TokenValidationParameters = tokenValidationParameters;
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["token"];
                    if (!string.IsNullOrEmpty(accessToken) /*&& context.Request.Path.StartsWithSegments("/wssonline")*/)
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
            {
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.NameIdentifier);
            });
        });
    }

    /// <summary>
    /// Configure application for current application
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public override void ConfigureApplication(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}