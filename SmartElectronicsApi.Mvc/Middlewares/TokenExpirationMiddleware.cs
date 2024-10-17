namespace SmartElectronicsApi.Mvc.Middlewares
{
    public class TokenExpirationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenExpirationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 401)
            {
                // Token has expired, log out the user
                context.Response.Cookies.Delete("jwtToken");

                // Redirect to the Home page instead of the login page
                context.Response.Redirect("/");
            }
        }
    }
}
