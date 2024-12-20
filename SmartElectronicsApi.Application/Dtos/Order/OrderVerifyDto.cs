using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Order
{
    public class OrderVerifyDto
    {
        public string UserName { get; set; }
        public int Id { get; set; }
        public string ShippedToken { get; set; }
    }
}
