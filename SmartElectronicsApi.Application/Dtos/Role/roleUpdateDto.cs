using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Role
{
    public class roleUpdateDto
    {
        public roleUpdateDto(IList<string> roles, string userName, List<IdentityRole> identityRoles)
        {
            this.roles = roles;
            UserName = userName;
            IdentityRoles = identityRoles;
        }
        public string UserName { get; set; }
        public List<IdentityRole> IdentityRoles { get; set; }
        public IList<string> roles { get; set; }
    }
}
