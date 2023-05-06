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


            modelBuilder.Entity<ApplicationUser>()
                .Property(b => b.IsSuspended)
                .HasDefaultValue(false);

            ////////////////////////////////
            /// //Product
            modelBuilder.Entity<Product>()
                .Property(b => b.IsSuspended)
                .HasDefaultValue(false);
            modelBuilder.Entity<Product>()
               .HasOne(p => p.Vendor)
               .WithMany(v => v.Products)
               .HasForeignKey(p => p.VendorId);

            //product Rate
            modelBuilder.Entity<ProductRate>()
            .HasKey(e => new { e.CustomerId, e.ProductId });

            modelBuilder.Entity<ProductRate>()
                .Property(b => b.IsPurchased)
                .HasDefaultValue(false);

            //wishList
            modelBuilder.Entity<WishList>()
           .HasKey(e => new { e.CustomerId, e.ProductId });


            // ShoppingCart
            modelBuilder.Entity<ShoppingCart>()
            .HasKey(e => new { e.CustomerId, e.ProductId });

            modelBuilder.Entity<ShoppingCart>()
                .HasOne(pq => pq.ApplicationUser)
                .WithMany(p => p.Carts)
                .HasForeignKey(pq => pq.CustomerId);

            modelBuilder.Entity<ShoppingCart>()
                .HasOne(pq => pq.Product)
                .WithMany(c => c.Carts)
                .HasForeignKey(pq => pq.ProductId);

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

            // ShoppingCart
            modelBuilder.Entity<Penalty>()
            .HasKey(e => new { e.AdminId, e.VendorId });

            modelBuilder.Entity<Penalty>()
                .HasOne(pq => pq.Admin)
                .WithMany(p => p.Penalties)
                .HasForeignKey(pq => pq.AdminId);

            modelBuilder.Entity<Penalty>()
                .HasOne(pq => pq.Vendor)
                .WithMany(c => c.Penalties)
                .HasForeignKey(pq => pq.VendorId);

            // ProdcutAction
            modelBuilder.Entity<ProdcutAction>()
            .HasKey(e => new { e.AdminId, e.ProductId });

            modelBuilder.Entity<ProdcutAction>()
                .HasOne(pq => pq.Admin)
                .WithMany(p => p.ProdcutsActions)
                .HasForeignKey(pq => pq.AdminId);

            modelBuilder.Entity<ProdcutAction>()
                .HasOne(pq => pq.Product)
                .WithMany(c => c.ProdcutsActions)
                .HasForeignKey(pq => pq.ProductId);


        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<WishList> WishesLists { get; set; }
        public DbSet<ProductRate> ProductsRates { get; set; }
        public DbSet<ProductQuestion> ProductsQuestions { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ProductReport> ProductsReports { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Penalty> Penalties { get; set; }
        public DbSet<ProdcutAction> ProdcutsActions { get; set; }
    }
}
