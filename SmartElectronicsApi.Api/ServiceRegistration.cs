using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartElectronicsApi.Api.Implementations;
using SmartElectronicsApi.Api.Interfaces;
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
            services.AddControllersWithViews();
            services.AddDbContext<SmartElectronicsDbContext>(options =>
              options.UseSqlServer(configuration.GetConnectionString("AppConnectionString"))
          );
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

        }
    }
}
