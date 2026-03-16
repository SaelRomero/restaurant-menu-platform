using MenuDigital.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MenuDigital.Infrastructure.Data
{
    public class MenuDigitalContext : DbContext
    {
        public MenuDigitalContext(DbContextOptions<MenuDigitalContext> options) : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Restaurant>()
                .HasIndex(r => r.Slug)
                .IsUnique();

            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasColumnType("decimal(18,2)");
                
            modelBuilder.Entity<MenuItem>()
                .Property(m => m.DiscountPercent)
                .HasColumnType("decimal(5,2)");
        }
    }
}
