using Dressify.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

            //Product
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
            modelBuilder.Entity<ShoppingCart>()
            .HasKey(e => new { e.CustomerId, e.ProductId });

            ////////////////////////////////
            modelBuilder.Entity<Product>()
               .HasOne(p => p.Vendor)
               .WithMany(v => v.Products)
               .HasForeignKey(p => p.VendorId);

            modelBuilder.Entity<Product>()
                .Property(b => b.Suspended)
                .HasDefaultValue(false); 

            //product rate
            modelBuilder.Entity<ProductRate>()
                .Property(b => b.IsPurchased)
                .HasDefaultValue(false);

            modelBuilder.Entity<ProductRate>()
            .HasKey(e => new { e.CustomerId, e.ProductId });

            //wishList
            modelBuilder.Entity<WishList>()
           .HasKey(e => new { e.CustomerId, e.ProductId });

            // Product Question
            modelBuilder.Entity<ProductQuestion>()
                .HasOne(pq => pq.Product)
                .WithMany(p => p.Questions)
                .HasForeignKey(pq => pq.ProductId);

            modelBuilder.Entity<ProductQuestion>()
                .HasOne(pq => pq.Customer)
                .WithMany(c => c.QuestionsAsked)
                .HasForeignKey(pq => pq.CustomerId);

            modelBuilder.Entity<ProductQuestion>()
                .HasOne(pq => pq.Vendor)
                .WithMany(v => v.QuestionsAnswered)
                .HasForeignKey(pq => pq.VendorId);

            modelBuilder.Entity<ProductQuestion>()
            .Property(pq => pq.QuestionDate)
            .HasDefaultValueSql("GETUTCDATE()");

            //ProductReport
            modelBuilder.Entity<ProductReport>()
                .HasOne(pq => pq.Product)
                .WithMany(p => p.Reports)
                .HasForeignKey(pq => pq.ProductId);

            modelBuilder.Entity<ProductReport>()
                .HasOne(pq => pq.Customer)
                .WithMany(c => c.Reports)
                .HasForeignKey(pq => pq.CustomerId);

            modelBuilder.Entity<ProductReport>()
                .HasOne(pq => pq.Admin)
                .WithMany(v => v.Reports)
                .HasForeignKey(pq => pq.AdminId);

            modelBuilder.Entity<ProductReport>()
            .Property(pq => pq.Date)
            .HasDefaultValueSql("GETUTCDATE()");


        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<WishList> WishesLists { get; set; }
        public DbSet<ProductRate> ProductsRates { get; set; }
        public DbSet<ProductQuestion> ProductsQuestions { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ProductReport> ProductsReports { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
    }
}
