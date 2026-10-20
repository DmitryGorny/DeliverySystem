using Microsoft.EntityFrameworkCore;
using Orders.Models;

namespace Orders.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Models.Orders> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Positions> Positions { get; set; }
    }
}
