using ChatGPT.Application.Auth.Commands;
using ChatGPT.Application.Auth.Dtos;
using ChatGPTtrading.Infrastructure;
using MediatR;

namespace ChatGPT.Application.Behaviors.Transaction;

public class SignUpCommandTransactionBehavior :
    TransactionBehavior<SignUpCommand, AuthResultVm>
{
    public SignUpCommandTransactionBehavior(ApplicationDbContext context)
        : base(context) { }
}