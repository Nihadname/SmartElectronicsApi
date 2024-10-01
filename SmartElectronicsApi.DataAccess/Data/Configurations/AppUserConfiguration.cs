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
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(s => s.UserName).HasMaxLength(100);
            builder.Property(s => s.fullName).HasMaxLength(150);
            builder.Property(s => s.Email).HasMaxLength(220);
            builder.Property(s=>s.IsBlocked).HasDefaultValue(false);
        }
    }
}
