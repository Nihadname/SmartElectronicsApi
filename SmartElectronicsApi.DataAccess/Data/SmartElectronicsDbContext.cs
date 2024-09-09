using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data
{
    public class SmartElectronicsDbContext:DbContext
    {
        public SmartElectronicsDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Category> categories { get; set; }
        public DbSet<SubCategory> subcategories { get; set; }
        public DbSet<Brand> brands { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }
        public DbSet<VariantOption> variantOptions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
