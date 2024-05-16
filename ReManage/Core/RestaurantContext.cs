using Microsoft.EntityFrameworkCore;
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
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<StorageModel> Storages { get; set; }

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
            modelBuilder.Entity<ProductModel>().ToTable("products");
            modelBuilder.Entity<EmployeeModel>().ToTable("employees");
            modelBuilder.Entity<RoleModel>().ToTable("roles");
            modelBuilder.Entity<StorageModel>().ToTable("storage");

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

            modelBuilder.Entity<RefrigeratorModel>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId);
        }
    }
}