using SmartElectronicsApi.Application.Dtos.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IDashboardService 
    {
        Task<SalesDataDto> GetWeeklySalesAsync();
        Task<SalesDataDto> GetMonthlySalesAsync();
        Task<SalesDataDto> GetYearlySalesAsync();
        Task<UserStatisticsDto> GetUserStatisticsAsync();
    }
}
