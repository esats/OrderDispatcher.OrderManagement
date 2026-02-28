using Microsoft.EntityFrameworkCore;
using OrderDispatcher.CatalogService.Entities;
using OrderDispatcher.OrderManagement.Entities;

namespace OrderDispatcher.CatalogService.Dal.Concrete.EntityFramework
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


            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer(@"Server=.;database=OrderDP.OrderManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true;Connect Timeout=150;TrustServerCertificate=True");
                //optionsBuilder.UseSqlServer(@"server=217.195.207.190;Initial Catalog=TexEx;User ID=Texexadmin;password=Texex2121**!!!");
            }

            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}


