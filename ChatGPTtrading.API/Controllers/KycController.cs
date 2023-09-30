using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatGPTtrading.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ChatGPT.Application.Kyc;
using ChatGPT.Application.Kyc.Dto;

namespace ChatGPTtrading.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public partial class KycController : ControllerBase
    {
        public record KycDataDto(string FirstName,
            string LastName,
            string Email,
            string Phone,
            string Residency,
            string Citizenship, string Occupation,
            string TemporaryResidence, string AvatarId,
            string PassportId,
            string PhotoWithPassportId);
        /// <summary>
        /// Загрузка данных для KYC
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> SendData([FromServices] IMediator mediator, [FromBody] KycDataDto dto)
        {
            var cmd = new AddKycDataCommand(Guid.Parse(User.Identity.Name),
                dto.FirstName,
                dto.LastName,
                dto.Email, dto.Phone,
                dto.Residency,
                dto.Citizenship,
                dto.Occupation,
                dto.TemporaryResidence,
                dto.AvatarId, dto.PassportId, dto.PhotoWithPassportId);
            var response = new BaseResponse<bool>(await mediator.Send(cmd));
            return Ok(response);
        }


        public record MediaItemDto(IFormFile File);
        /// <summary>
        /// Загрузить файл
        /// </summary>
        /// <returns>ИД загруженного файла</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<Guid>), 200)]
        public async Task<IActionResult> UploadFile([FromServices] IMediator mediator, [FromForm] MediaItemDto dto)
        {
            var response = new BaseResponse<Guid>(await mediator.Send(new UploadFileCommand(dto.File)));
            return Ok(response);
        }

        /// <summary>
        /// Получить данные при регистрации
        /// </summary>
        /// <returns>емейл и телефон</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<RegistrationDataVm>), 200)]
        public async Task<IActionResult> GetKnownData([FromServices] IMediator mediator)
        {
            var query = new GetRegistrationDataQuery(Guid.Parse(User.Identity.Name));
            var response = new BaseResponse<RegistrationDataVm>(await mediator.Send(query));
            return Ok(response);
        } 
        
        /// <summary>
        /// Нужно ли подтвердить KYC
        /// </summary>
        /// <returns>вернёт true если KYC надо подтвердить</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        public async Task<IActionResult> GetIsNeedSendKyc([FromServices] IMediator mediator)
        {
            var query = new GetIsNeedSendKycQuery(Guid.Parse(User.Identity.Name));
            var response = new BaseResponse<bool>(await mediator.Send(query));
            return Ok(response);
        }
    }
}
