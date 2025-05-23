﻿using SmartElectronicsApi.Core.Entities;

namespace SmartElectronicsApi.Mvc.ViewModels.Address
{
    public class AddressCreateVm
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public AddressType AddressType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AppUserId { get; set; }
    }
}
