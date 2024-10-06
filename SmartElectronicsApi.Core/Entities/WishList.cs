using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class WishList:BaseEntity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<WishListProduct> wishListProducts { get; set; }
    }
}
