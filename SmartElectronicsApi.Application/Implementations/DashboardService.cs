using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos.Dashboard;
using SmartElectronicsApi.Application.Extensions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.DataAccess.Data;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly SmartElectronicsDbContext _context;

        public DashboardService(SmartElectronicsDbContext context)
        {
            _context = context;
        }

        public async Task<SalesDataDto> GetWeeklySalesAsync()
        {
            var currentWeekStart = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            var previousWeekStart = currentWeekStart.AddDays(-7);

            var currentWeekSales = await _context.Orders
                .Where(o => o.OrderDate >= currentWeekStart)
                .SumAsync(o => o.TotalAmount);

            var previousWeekSales = await _context.Orders
                .Where(o => o.OrderDate >= previousWeekStart && o.OrderDate < currentWeekStart)
                .SumAsync(o => o.TotalAmount);

            return new SalesDataDto
            {
                TotalSales = currentWeekSales,
                PreviousPeriodSales = previousWeekSales,
                PercentageChange = CalculatePercentageChange(previousWeekSales, currentWeekSales)
            };
        }
        public async Task<SalesDataDto> GetMonthlySalesAsync()
        {
            var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);

            var currentMonthSales = await _context.Orders
                .Where(o => o.OrderDate >= currentMonthStart)
                .SumAsync(o => o.TotalAmount);

            var previousMonthSales = await _context.Orders
                .Where(o => o.OrderDate >= previousMonthStart && o.OrderDate < currentMonthStart)
                .SumAsync(o => o.TotalAmount);

            return new SalesDataDto
            {
                TotalSales = currentMonthSales,
                PreviousPeriodSales = previousMonthSales,
                PercentageChange = CalculatePercentageChange(previousMonthSales, currentMonthSales)
            };
        }

        public async Task<SalesDataDto> GetYearlySalesAsync()
        {
            var currentYearStart = new DateTime(DateTime.Now.Year, 1, 1);
            var previousYearStart = currentYearStart.AddYears(-1);

            var currentYearSales = await _context.Orders
                .Where(o => o.OrderDate >= currentYearStart)
                .SumAsync(o => o.TotalAmount);

            var previousYearSales = await _context.Orders
                .Where(o => o.OrderDate >= previousYearStart && o.OrderDate < currentYearStart)
                .SumAsync(o => o.TotalAmount);

            return new SalesDataDto
            {
                TotalSales = currentYearSales,
                PreviousPeriodSales = previousYearSales,
                PercentageChange = CalculatePercentageChange(previousYearSales, currentYearSales)
            };
        }
        public async Task<UserStatisticsDto> GetUserStatisticsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var newUsersThisMonth = await _context.Users
                .Where(u => u.CreatedTime >= currentMonthStart)
                .CountAsync();

            var currentWeekStart = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            var newUsersThisWeek = await _context.Users
                .Where(u => u.CreatedTime >= currentWeekStart)
                .CountAsync();

            return new UserStatisticsDto
            {
                TotalUsers = totalUsers,
                NewUsersThisWeek = newUsersThisWeek,
                NewUsersThisMonth = newUsersThisMonth
            };
        }

        private decimal CalculatePercentageChange(decimal previousPeriodSales, decimal currentPeriodSales)
        {
            if (previousPeriodSales == 0) return 100;
            return ((currentPeriodSales - previousPeriodSales) / previousPeriodSales) * 100;
        }
    }
}
