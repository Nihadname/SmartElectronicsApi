using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Address
{
    public class AddressListItemDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string AddressType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public UserGetDto AppUser { get; set; }

    }
}
