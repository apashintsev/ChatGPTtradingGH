using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatGPTtrading.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ChatGPT.Application.Balance;
using ChatGPT.Application.Payments;
using ChatGPT.Application.Payments.Dtos;
using ChatGPT.Application.Payments.Commands;
using ChatGPT.Application.Payments.Queries;

namespace ChatGPTtrading.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class PaymentsController : ControllerBase
    {

        /// <summary>
        /// Reinvest
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<BalanceVm>), 200)]
        public async Task<IActionResult> Reinvest([FromServices] IMediator mediator)
        {
            var cmd = new ReinvestCommand(Guid.Parse(User.Identity.Name));
            var response = new BaseResponse<BalanceVm>(await mediator.Send(cmd));
            return Ok(response);
        }

        /// <summary>
        /// Получить общий баланс пользователя
        /// </summary>
        /// <returns>сколько инвестировано и сколько заработано профита</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<BalanceVm>), 200)]
        public async Task<IActionResult> GetBalance([FromServices] IMediator mediator)
        {
            var query = new GetBalancesQuery(Guid.Parse(User.Identity.Name));
            var response = new BaseResponse<BalanceVm>(await mediator.Send(query));
            return Ok(response);
        }

        /// <summary>
        /// Получить выводы пользователя
        /// </summary>
        /// <returns>список выводов</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IList<WithdrawalVm>>), 200)]
        public async Task<IActionResult> GetWithdrawals([FromServices] IMediator mediator)
        {
            var query = new GetWithdrawalsQuery(Guid.Parse(User.Identity.Name));
            var response = new BaseResponse<IList<WithdrawalVm>>(await mediator.Send(query));
            return Ok(response);
        }

        /// <summary>
        /// Deposit for test
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<decimal>), 200)]
        public async Task<IActionResult> Deposit([FromServices] IMediator mediator)
        {
            var cmd = new DepositCommand(Guid.Parse(User.Identity.Name), 100);
            var response = new BaseResponse<decimal>(await mediator.Send(cmd));
            return Ok(response);
        }

        public record WithdrawalReq(AccountType AccountType, decimal Amount, string Address, WithdrawNetwork Network);
        /// <summary>
        /// Создать заявку на вывод
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<WithdrawalVm>), 200)]
        public async Task<IActionResult> CreateWithdrawRequest([FromServices] IMediator mediator, [FromBody] WithdrawalReq dto)
        {
            var cmd = new CreateWithdrawRequestCommand(Guid.Parse(User.Identity.Name), dto.AccountType, dto.Amount, dto.Address, dto.Network);
            var response = new BaseResponse<WithdrawalVm>(await mediator.Send(cmd));
            return Ok(response);
        }

        /// <summary>
        /// Получить ссылку на форму оплаты
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<string>), 200)]
        public async Task<IActionResult> GetDepositUrl([FromServices] IMediator mediator, decimal amount/*, string currency*/)
        {
            var cmd = new GetDepositUrlQuery(Guid.Parse(User.Identity.Name), amount/*, currency*/);
            var response = new BaseResponse<string>(await mediator.Send(cmd));
            return Ok(response);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CallbackUrl([FromServices] IMediator mediator, Guid id)
        {
            var cmd = new DepositCallbackCommand(id);
            var response = new BaseResponse<decimal>(await mediator.Send(cmd));
            return Ok(response);
        }
    }
}
