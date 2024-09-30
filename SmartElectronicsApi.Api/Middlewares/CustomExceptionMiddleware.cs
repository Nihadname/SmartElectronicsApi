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
                var errors = new Dictionary<string, string>();
                var responseMessage = ex.Message;

                // If it's a CustomException, extract the errors
                if (ex is CustomException customException)
                {
                    errors = customException.Errors;
                    responseMessage = customException.Message;
                }

                var response = new
                {
                    message = responseMessage,
                    errors = errors // Always include the errors dictionary, even if empty
                };

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = ex is CustomException custom
                    ? custom.Code
                    : StatusCodes.Status500InternalServerError;

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
