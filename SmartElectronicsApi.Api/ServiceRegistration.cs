using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartElectronicsApi.Application.Implementations;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Repositories;
using SmartElectronicsApi.DataAccess.Data;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;

namespace SmartElectronicsApi.Api
{
    public static class ServiceRegistration
    {
        public static void Register(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews()
                .ConfigureApiBehaviorOptions(opt =>
            {
                opt.InvalidModelStateResponseFactory = context =>
                {
                    // Convert validation errors to a consistent dictionary format
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count() > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value.Errors.First().ErrorMessage
                        );

                    // Return a consistent response format with an empty message
                    return new BadRequestObjectResult(new
                    {
                        message = "",
                        errors
                    });
                };
            });
            services.AddDbContext<SmartElectronicsDbContext>(options =>
              options.UseSqlServer(configuration.GetConnectionString("AppConnectionString"))
          );
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = configuration["Authentication:Google:ClientId"];
    options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google"; // Ensure this matches the redirect URI set in Google Cloud
    options.SaveTokens = true;
    options.Scope.Add("email");
    options.Scope.Add("profile");
});
        }
    }
}
