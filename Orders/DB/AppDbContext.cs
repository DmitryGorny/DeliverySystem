using Microsoft.EntityFrameworkCore;
using Orders.Models;

namespace Orders.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        DbSet<Models.Orders> Orders { get; set; }
        DbSet<OrderStatus> OrderStatuses { get; set; }
        DbSet<Positions> Positions { get; set; }
    }
}
