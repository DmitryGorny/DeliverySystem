using Microsoft.EntityFrameworkCore;
using Orders.Common.Constants;
using Orders.Models;
using System.Reflection.Emit;

namespace Orders.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Models.Orders> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Positions> Positions { get; set; }
        public DbSet<OrdersFailure> OrdersFailures { get; set; }
        public DbSet<FailurePositions> FailurePositions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.OrderStatus>(b =>
            {
                b.HasData(
                    Enum.GetValues(typeof(OrderStatusEnum)).Cast<OrderStatusEnum>()
                        .Select(s => new Models.OrderStatus { Id = (int)s, Status = s.ToString() }));
            });

            modelBuilder.Entity<Models.Orders>(b =>
            {
                b.HasIndex(o => new { o.OrderStatusId, o.CreatedAt });
            });
        }
    }
}
