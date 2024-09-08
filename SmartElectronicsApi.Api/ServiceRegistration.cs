using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartElectronicsApi.DataAccess.Data;
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
        }
    }
}
