using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatGPTtrading.Models;
using ChatGPT.Application.Auth.Dtos;
using ChatGPT.Application.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Swashbuckle.AspNetCore.Annotations;

namespace ChatGPTtrading.API.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public partial class AuthController : ControllerBase
{
    /// <summary>
    /// Admins email
    /// </summary>
    /// <param name="Email" example="admin@example.com">Pochta</param>
    /// <param name="Password"></param>
    public record UserSignInDto(string Email, string Password);
    /// <summary>
    /// Логин по Email и паролю
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<AuthResultVm>), 200)]
    public async Task<IActionResult> Login([FromServices] IMediator mediator, [FromBody] UserSignInDto dto)
    {
        var cmd = new LoginCommand(dto.Email, dto.Password);
        var response = new BaseResponse<AuthResultVm>(await mediator.Send(cmd));
        return Ok(response);
    }

    public record UserSignUpDto(string Email, string Phone, string Password, string Referral);
    /// <summary>
    /// Регистрация с использованием почты и телефона
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<AuthResultVm>), 200)]
    public async Task<IActionResult> SignUp([FromServices] IMediator mediator, [FromBody] UserSignUpDto dto)
    {
        var cmd = new SignUpCommand(dto.Email, dto.Phone, dto.Password, dto.Referral);
        var response = new BaseResponse<AuthResultVm>(await mediator.Send(cmd));
        return Ok(response);
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> TestAuth()
    {
        string nameId = User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
        return Ok("Authorized: "+nameId);
    }

    public record ConfirmEmailDto(string Email, string Code);
    /// <summary>
    /// Подтвердить почту введением кода
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
    public async Task<IActionResult> СonfirmEmail([FromServices] IMediator mediator, [FromBody] ConfirmEmailDto dto)
    {
        var command = new ConfirmEmailCommand(dto.Email, dto.Code);
        var response = new BaseResponse<bool>(await mediator.Send(command));
        return Ok(response);
    }
}
