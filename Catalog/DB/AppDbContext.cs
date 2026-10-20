using Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
    }
}
