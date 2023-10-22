using Daleya.API.Models;
using Daleya.API.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Daleya.API.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<IdentityRole>  IdentityRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                        .HasMany(c => c.Products)
                        .WithOne(p => p.Category)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>().HasData(new Category { CategoryId = 2, Name = "Drinks" },
                new Category { CategoryId = 1, Name = "Appetizers" },
                new Category { CategoryId = 3, Name = "Desserts" });

            //modelBuilder.Entity<IdentityRole>().HasData(
            //   new IdentityRole { Name = SD.RoleAdmin, NormalizedName = SD.RoleAdmin },
            //   new IdentityRole { Name = SD.RoleCustomer, NormalizedName = SD.RoleCustomer });

        }
    }
}
