using Microsoft.EntityFrameworkCore;
using Payment.Models;

namespace Payment.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { }

        public DbSet<CreatedPayments> CreatedPayments { get; set; }
    }
}
