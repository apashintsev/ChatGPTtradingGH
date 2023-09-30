using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatGPTtrading.Models;
using ChatGPT.Application.Statistics.Dto;
using ChatGPT.Application.Statistics;
using ChatGPT.Application.Statistics.Queries;

namespace ChatGPTtrading.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public partial class StatsController : ControllerBase
{
    /// <summary>
    /// Получить статистику на лендос
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<StatTotalVm>), 200)]
    public async Task<IActionResult> GetStat([FromServices] IMediator mediator)
    {
        var query = new GetFakeStatQuery();
        var response = new BaseResponse<StatTotalVm>(await mediator.Send(query));
        return Ok(response);
    }

    /// <summary>
    /// Получить активности
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<IEnumerable<ActivityVm>>), 200)]
    public async Task<IActionResult> GetActivities([FromServices] IMediator mediator)
    {
        var query = new GetActivitiesQuery();
        var response = new BaseResponse<IEnumerable<ActivityVm>>(await mediator.Send(query));
        return Ok(response);
    }

}
