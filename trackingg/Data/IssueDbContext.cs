using Microsoft.EntityFrameworkCore;
using trackingg.Models;

namespace trackingg.Data
{
    public class IssueDbContext : DbContext
    {
        public IssueDbContext(DbContextOptions<IssueDbContext> options) : base(options)
        {
        }

        public DbSet<Issue> Issues { get; set; } = null!;
    }
}