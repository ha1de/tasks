using Microsoft.EntityFrameworkCore;
using trackingg.Models;

namespace trackingg.Data
{
    // This class is now redundant as all issues are in ApplicationDbContext
    // But we'll keep it for backward compatibility
    public class IssueDbContext : DbContext
    {
        public IssueDbContext(DbContextOptions<IssueDbContext> options) : base(options)
        {
        }

        public DbSet<Issue> Issues { get; set; } = null!;
    }
}