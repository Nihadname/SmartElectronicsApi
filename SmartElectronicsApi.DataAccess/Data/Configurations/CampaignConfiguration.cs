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
            builder.Property(s => s.DiscountPercentage).IsRequired(true).HasColumnType("decimal(18, 2)");
            builder.HasKey(e => e.Id);
        }
    }
}
