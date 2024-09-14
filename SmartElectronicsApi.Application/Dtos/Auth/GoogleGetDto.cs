using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Auth
{
    public class GoogleGetDto
    {
        public string Email { get; set; }
        public string userName { get; set; }
        public string Id    { get; set; }
        public string GivenName { get; set; }
    }
}
