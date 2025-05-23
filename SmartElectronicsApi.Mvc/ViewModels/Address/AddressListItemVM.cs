﻿using SmartElectronicsApi.Mvc.ViewModels.Auth;

namespace SmartElectronicsApi.Mvc.ViewModels.Address
{
    public class AddressListItemVM
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string AddressType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public UserGetVm AppUser { get; set; }
    }
}
