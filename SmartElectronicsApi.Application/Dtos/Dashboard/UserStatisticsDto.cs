using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Dashboard
{
    public class UserStatisticsDto
    { 
        public int TotalUsers { get; set; }
        public int NewUsersThisWeek { get; set; }
        public int NewUsersThisMonth { get; set; }
    }
}
