
using ChatGPTtrading.Models;
using Newtonsoft.Json;
using System.Net;

namespace ChatGPTtrading.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext,
                    ex,
                    HttpStatusCode.BadRequest);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode httpStatusCode)
        {
            _logger.LogError(ex.TargetSite.Name, ex);

            HttpResponse response = context.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)httpStatusCode;

            BaseResponse<object> resp = new(null);
            resp.AddError(ex.Message);

            var json = JsonConvert.SerializeObject(resp);
            await response.WriteAsync(json);
        }
    }
}