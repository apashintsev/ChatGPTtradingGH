﻿using ChatGPT.Application.Auth.Commands;
using ChatGPTtrading.API.Definitions.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ChatGPTtrading.API.Definitions.FluentValidating;

/// <summary>
/// FluentValidation registration as Application definition
/// </summary>
public class FluentValidationDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);
    }
}