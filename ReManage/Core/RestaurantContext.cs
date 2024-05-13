using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReManage.Models;
using NodaTime;

namespace ReManage.Core
{
    public class RestaurantContext : DbContext
    {
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderedDishModel> OrderedDishes { get; set; }
        public DbSet<OrderStatusModel> OrderStatuses { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<DishModel> Dishes { get; set; }
        public DbSet<CompositionModel> Compositions { get; set; }
        public DbSet<RefrigeratorModel> Refrigerators { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=ReManage;User Id=postgres;Password=35x45x");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderModel>().ToTable("orders");
            modelBuilder.Entity<OrderedDishModel>().ToTable("ordered_dishes");
            modelBuilder.Entity<OrderStatusModel>().ToTable("order_status");
            modelBuilder.Entity<CategoryModel>().ToTable("category");
            modelBuilder.Entity<DishModel>().ToTable("dishes");
            modelBuilder.Entity<CompositionModel>().ToTable("composition");
            modelBuilder.Entity<RefrigeratorModel>().ToTable("refrigerator");

            modelBuilder.Entity<OrderModel>()
                .Property(o => o.id)
                .ValueGeneratedOnAdd();

            var periodConverter = new PeriodConverter();

            modelBuilder.Entity<RefrigeratorModel>()
                .Property(r => r.ShelfLife)
                .HasConversion(periodConverter);

            modelBuilder.Entity<RefrigeratorModel>()
                .Property(r => r.UnfreezeTime)
                .HasConversion(periodConverter);
        }
    }
}