using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Auth
{
    public class UserGetDto
    {
        public string FullName { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
