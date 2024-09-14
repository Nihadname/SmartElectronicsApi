using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Auth
{
    public class LoginDto
    {
        public string UserNameOrGmail { get; set; }
        public string Password { get; set; }
    }
}
