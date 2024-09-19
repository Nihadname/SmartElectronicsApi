using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Auth
{
    public class ResetPasswordEmailDto
    {
        public string Email { get; set; }   
        public string? Token { get; set; }
    }
}
