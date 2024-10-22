using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public class AppUser: IdentityUser
    {
        public string fullName { get; set; }
        public string? GoogleId { get; set; }
        public string? Image {  get; set; }
        public bool IsBlocked { get; set; }
        public int? loyalPoints { get; set; }
        public int? loyaltyTier { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? BlockedUntil { get; set; }
        public ICollection<Address>? addresses { get; set; }
        public Basket? basket { get; set; }
        public WishList? wishList { get; set; }
        public ICollection<Comment>? comments { get; set; }  
        public ICollection<Order>? orders { get; set; }

    }

}
