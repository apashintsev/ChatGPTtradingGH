using ChatGPTtrading.API.Definitions.Common;
using ChatGPTtrading.Middlewares;
using ChatGPTtrading.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddConfigMapping(builder.Configuration);
// Add services to the container.
builder.Services.AddDefinitions(builder, typeof(Program));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseDefinitions();

app.Run();
