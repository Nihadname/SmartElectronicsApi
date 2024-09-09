using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(o => o.Price).IsRequired(true).HasColumnType("decimal(18, 2)");
            builder.Property(o => o.DiscountPercentage).IsRequired(true).HasColumnType("decimal(18, 2)");
            builder.Property(s=>s.DiscountedPrice).IsRequired(true).HasColumnType("decimal(18, 2)");

        }
    }
}
