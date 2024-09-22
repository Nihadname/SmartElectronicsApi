using SmartElectronicsApi.Application.Exceptions;

namespace SmartElectronicsApi.Api.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException.Message,
                    errors = new Dictionary<string, string>()
                };

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = ex is CustomException custom
                    ? custom.Code
                    : StatusCodes.Status500InternalServerError;

                if (ex is CustomException customException)
                {
                    response = new
                    {
                        message = customException.Message,
                        errors = customException.Errors
                    };
                }

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }
    }
}