using SmartElectronicsApi.Application.Exceptions;

namespace SmartElectronicsApi.Api.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _Next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _Next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _Next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var errors = new Dictionary<string, string>();
                httpContext.Response.StatusCode = 500;
                if (ex is CustomException custom)
                {
                    message = custom.Message;
                    errors = custom.Errors;
                    httpContext.Response.StatusCode = custom.Code;

                }
                await httpContext.Response.WriteAsJsonAsync(new { message, errors });


            }
        }
    }
}
