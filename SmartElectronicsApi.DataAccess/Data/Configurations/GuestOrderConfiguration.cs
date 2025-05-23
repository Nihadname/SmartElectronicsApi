﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartElectronicsApi.Core.Entities;

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
            
            
            builder.Property(s => s.ProductPrice).IsRequired(true).HasColumnType("decimal(18, 2)");
           
        }
    }
}
