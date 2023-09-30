using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatGPTtrading.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ChatGPT.Application.Balance;
using ChatGPT.Application.Referals;
using ChatGPT.Application.Referals.Dtos;

namespace ChatGPTtrading.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class ReferalsController : ControllerBase
    {
        /// <summary>
        /// Получить реферальную ссылку пользователя
        /// </summary>
        /// <returns>ссылка</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<string>), 200)]
        public async Task<IActionResult> GetRefLink([FromServices] IMediator mediator)
        {
            var query = new GetRefLinkQuery(Guid.Parse(User.Identity.Name));
            var response = new BaseResponse<string>(await mediator.Send(query));
            return Ok(response);
        }

        /// <summary>
        /// Получить количество рефералов
        /// </summary>
        /// <returns>ссылка</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<int>), 200)]
        public async Task<IActionResult> GetRefCount([FromServices] IMediator mediator)
        {
            var query = new GetReferalsCountQuery(Guid.Parse(User.Identity.Name));
            var response = new BaseResponse<int>(await mediator.Send(query));
            return Ok(response);
        }

        /// <summary>
        /// Получить данные по рефералам за период
        /// </summary>
        /// <returns>ссылка</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<RefRewardsVm>), 200)]
        public async Task<IActionResult> GetRefRewards([FromServices] IMediator mediator, DateTime start, DateTime end)
        {
            var query = new GetRefRewardsQuery(Guid.Parse(User.Identity.Name), start, end);
            var response = new BaseResponse<RefRewardsVm>(await mediator.Send(query));
            return Ok(response);
        }

        /// <summary>
        /// Получить реф настройки
        /// </summary>
        /// <returns>ссылка</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<RefSettingsVm>), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> GetRefSettings([FromServices] IMediator mediator)
        {
            var query = new GetRefSettingsQuery();
            var response = new BaseResponse<RefSettingsVm>(await mediator.Send(query));
            return Ok(response);
        }

    }
}
