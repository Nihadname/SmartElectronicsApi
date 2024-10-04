using Microsoft.AspNetCore.Identity;

namespace SmartElectronicsApi.Mvc.ViewModels.Auth
{
    public class RoleUpdateVM
    {
        public string UserName { get; set; }
        public List<IdentityRole> IdentityRoles { get; set; }
        public IList<string> roles { get; set; }
    }
}
