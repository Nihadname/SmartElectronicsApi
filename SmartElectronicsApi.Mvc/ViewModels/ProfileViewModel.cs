using SmartElectronicsApi.Mvc.ViewModels.Address;
using SmartElectronicsApi.Mvc.ViewModels.Auth;

namespace SmartElectronicsApi.Mvc.ViewModels
{
    public class ProfileViewModel
    {
        public UserGetVm UserGetVm { get; set; }
        public List<AddressListItemVM> AddressList { get; set; }
    }
}
