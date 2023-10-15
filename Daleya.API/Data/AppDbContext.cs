using Daleya.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Daleya.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>()
                        .HasMany(c => c.Products)
                        .WithOne(p => p.Category)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
