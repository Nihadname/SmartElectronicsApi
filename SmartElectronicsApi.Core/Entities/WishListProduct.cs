using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class WishListProduct:BaseEntity
    {
        public int Quantity { get; set; }
        public int WishListId { get; set; }
        public WishList  WishList { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
