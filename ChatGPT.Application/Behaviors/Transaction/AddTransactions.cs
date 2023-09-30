using ChatGPT.Application.Auth.Commands;
using ChatGPT.Application.Auth.Dtos;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGPT.Application.Behaviors.Transaction
{
    public static class AddTransactions
    {
        public static void AddTransactionsBehavior(this IServiceCollection services)
        {
            services.AddTransient<IPipelineBehavior<SignUpCommand, AuthResultVm>, SignUpCommandTransactionBehavior>();

        }
    }
}
