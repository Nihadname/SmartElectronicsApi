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
    public class GuestOrderConfiguration : IEntityTypeConfiguration<GuestOrder>
    {
        public void Configure(EntityTypeBuilder<GuestOrder> builder)
        {
            builder.Property(s => s.IsDeleted).HasDefaultValue(false);
            builder.Property(s => s.CreatedTime).HasDefaultValueSql("GETDATE()");
            builder.Property(s => s.UpdatedTime).HasDefaultValueSql("GETDATE()");
            builder.HasKey(e => e.Id);
            builder.Property(s => s.PurchasedProductId).IsRequired(false);
            builder.Property(s => s.PurchasedProducVariationtId).IsRequired(false);
            builder.HasIndex(p => p.FullName)
            .IsUnique();
            builder.HasIndex(p => p.PhoneNumber)
            .IsUnique();
            builder.Property(s => s.ProductPrice).IsRequired(true).HasColumnType("decimal(18, 2)");
            builder.HasIndex(p => p.EmailAdress)
            .IsUnique();
        }
    }
}
