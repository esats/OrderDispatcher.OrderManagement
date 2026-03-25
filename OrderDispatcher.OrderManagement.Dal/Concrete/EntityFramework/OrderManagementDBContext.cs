using Microsoft.EntityFrameworkCore;
using OrderDispatcher.CatalogService.Entities;
using OrderDispatcher.OrderManagement.Entities;

namespace OrderDispatcher.OrderManagement.Dal.Concrete.EntityFramework
{
    public class OrderManagementDBContext : DbContext
    {
        public virtual DbSet<BasketMaster> BasketMasters { get; set; }
        public virtual DbSet<BasketDetail> BasketDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        public OrderManagementDBContext(DbContextOptions<OrderManagementDBContext> options) : base(options)
        {
        }

        public OrderManagementDBContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}


