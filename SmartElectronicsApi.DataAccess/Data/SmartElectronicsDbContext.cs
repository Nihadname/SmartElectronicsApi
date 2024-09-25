using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Core;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data
{
    public class SmartElectronicsDbContext:IdentityDbContext<AppUser>
    {
        public SmartElectronicsDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Category> categories { get; set; }
        public DbSet<SubCategory> subcategories { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }
        public DbSet<Color> colors { get; set; }
        public DbSet<ParametrGroup> parametrGroups { get; set; }
        public DbSet<ParametrValue> parametrValues { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductColor> productColors    { get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        public DbSet<ProductVariationColor> productVariationColors { get; set; }
        public DbSet<Slider> sliders { get; set; }
        public DbSet<Subscriber> subscribers { get; set; }
        public DbSet<Setting> settings { get; set; }
        public DbSet<BrandSubCategory> brandSubCategories { get; set; }
        public DbSet<Address> addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
