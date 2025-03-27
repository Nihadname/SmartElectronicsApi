using SmartElectronicsApi.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Core.Entities
{
    public sealed class Campaign:BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }   
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get;  set; }
        public decimal DiscountPercentage { get; set; }
        public ICollection<Product> Products { get; set; }

    }
    public enum Status
    {
        Active,
        Inactive,
    }
}
