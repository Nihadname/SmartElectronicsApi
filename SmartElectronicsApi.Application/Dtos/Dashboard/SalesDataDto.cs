using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Dashboard
{
    public class SalesDataDto
    {
        public decimal TotalSales { get; set; }
        public decimal PreviousPeriodSales { get; set; }
        public decimal PercentageChange { get; set; }
    }
}
