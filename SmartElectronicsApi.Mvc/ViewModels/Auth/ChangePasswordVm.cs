namespace SmartElectronicsApi.Mvc.ViewModels.Auth
{
    public class ChangePasswordVm
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
