using SmartElectronicsApi.Mvc.Interfaces;
using SmartElectronicsApi.Mvc.Middlewares;
using SmartElectronicsApi.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmailService, EmailService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Enable Developer Exception Page for detailed error information
    app.UseDeveloperExceptionPage();

    // Add middleware to handle 404 errors in development
    app.Use(async (context, next) =>
    {
        await next();

        if (context.Response.StatusCode == 404)
        {
            // Redirect to a custom 404 page
            context.Response.Redirect("/Error/404");
        }
    });
}
else
{
    // Add a custom exception handler for other environments
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<TokenExpirationMiddleware>();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
           name: "areas",
           pattern: "{area:exists}/{controller=Dasboard}/{action=Index}/{id?}"
         );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
