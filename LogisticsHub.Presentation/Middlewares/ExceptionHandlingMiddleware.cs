using System.Net;
using System.Text.Json;

namespace LogisticsHub.Presentation.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex,$"An Error has occured : {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "An Unexcepcted Error has occured ,Please try later..",
                Detail = ex.Message,
            };

            var options=new JsonSerializerOptions { PropertyNamingPolicy= JsonNamingPolicy.CamelCase };

            var json=JsonSerializer.Serialize(response, options);

             await context.Response.WriteAsync(json);
        }
    }
}
