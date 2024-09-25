using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class Address:BaseEntity
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public AddressType AddressType { get; set; }

        public string AppUserId { get; set; }
        public AppUser appUser  { get; set; }
    }
    public enum AddressType
    {
        Home,
        Work,
        Billing,
        Shipping,
        Other
    }
}
