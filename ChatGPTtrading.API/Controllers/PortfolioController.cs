using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatGPTtrading.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ChatGPT.Application.Portfolios.Dtos;
using ChatGPT.Application.Portfolios;

namespace ChatGPTtrading.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class PortfolioController : ControllerBase
    {
        /// <summary>
        /// Получить портфель отсортированный по процентам
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PortfolioVm>>), 200)]
        public async Task<IActionResult> GetPortfolio([FromServices] IMediator mediator)
        {
            var query = new GetPortfolioQuery();
            var response = new BaseResponse<IEnumerable<PortfolioVm>>(await mediator.Send(query));
            return Ok(response);
        }

        /// <summary>
        /// Получить статистику инвестирования
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<StatisticsVm>), 200)]
        public async Task<IActionResult> GetStatistics([FromServices] IMediator mediator, string dateFilter)
        {
            var query = new GetStatisticsQuery(Guid.Parse(User.Identity.Name),dateFilter);
            var response = new BaseResponse<StatisticsVm>(await mediator.Send(query));
            return Ok(response);
        }

    }
}
