using Dressify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Xml;


namespace Dressify.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            modelBuilder.Entity<Product>()
                .Property(b => b.Suspended)
                .HasDefaultValue(false); 

            modelBuilder.Entity<ProductRate>()
                .Property(b => b.IsPurchased)
                .HasDefaultValue(false);

            modelBuilder.Entity<WishList>()
            .HasKey(e => new { e.CustomerId, e.ProductId });
            modelBuilder.Entity<ProductRate>()
            .HasKey(e => new { e.CustomerId, e.ProductId });
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<WishList> WishesLists { get; set; }
        public DbSet<ProductRate> ProductsRates { get; set; }
    }
}
