using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Address
{
    public class AddressCreateDto:IAddressDto
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
