using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Auth
{
    public class ResetPasswordDto
    {
        public string Password { get; set; }    
        public string RePassword { get; set; }
    }
}
