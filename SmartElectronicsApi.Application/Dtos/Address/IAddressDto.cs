using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.Address
{
    public interface IAddressDto
    {
        string Country { get; set; }
        string State { get; set; }
        string City { get; set; }
        string Street { get; set; }

    }
}
