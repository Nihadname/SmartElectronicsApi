using System.ComponentModel.DataAnnotations;

namespace SmartElectronicsApi.Mvc.ViewModels.Auth
{
    public class ResetPasswordVm
    {
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}
