using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartElectronicsApi.Core.Entities;

namespace SmartElectronicsApi.DataAccess.Data.Configurations
{
    public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.Property(s => s.IsDeleted).HasDefaultValue(false);
            builder.Property(s => s.CreatedTime).HasDefaultValueSql("GETDATE()");
            builder.Property(s => s.UpdatedTime).HasDefaultValueSql("GETDATE()");
            builder.Property(s => s.DiscountPercentageValue).HasColumnType("decimal(18, 2)").HasDefaultValue(0m);
            builder.HasKey(e => e.Id);
            builder.Property(s=>s.Title).HasMaxLength(120);
            builder.Property(s=>s.Description).HasMaxLength(180);
            builder.Property(s=>s.StartDate).IsRequired(true);
            builder.Property(s=>s.EndDate).IsRequired(true);
            
        }
    }
}
