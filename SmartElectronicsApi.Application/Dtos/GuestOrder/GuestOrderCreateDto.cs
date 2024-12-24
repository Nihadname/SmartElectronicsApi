using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Dtos.GuestOrder
{
    public class GuestOrderCreateDto
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string EmailAdress { get; set; }
        public string PhoneNumber { get; set; }
        public string ExtraInformation { get; set; }
        public bool IsGottenFromStore { get; set; }
        public int PurchasedProductId { get; set; }
    }
}
