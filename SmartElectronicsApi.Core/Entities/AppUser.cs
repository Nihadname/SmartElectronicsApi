using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class AppUser: IdentityUser
    {
        public string fullName { get; set; }
        public string? GoogleId { get; set; }
        public string? Image {  get; set; }

    }
}
