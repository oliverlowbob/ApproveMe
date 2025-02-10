using Microsoft.EntityFrameworkCore;
using ApproveMe.Models.Transactions;

namespace ApproveMe.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Transaction> Transactions { get; set; }
    }
}