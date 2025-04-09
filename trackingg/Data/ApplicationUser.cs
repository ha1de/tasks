using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using trackingg.Models;

namespace trackingg.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }
        public virtual ICollection<Issue> AssignedIssues { get; set; } = new List<Issue>();
        public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
        public virtual ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Issue> Issues { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Activity> Activities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Issue>()
                .HasMany(i => i.Tags)
                .WithMany(t => t.Issues)
                .UsingEntity(j => j.ToTable("IssueTags"));

            builder.Entity<Issue>()
                .HasOne(i => i.AssignedTo)
                .WithMany(u => u.AssignedIssues)
                .HasForeignKey(i => i.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Issue>()
                .HasOne(i => i.Project)
                .WithMany(p => p.Issues)
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "ProjectManager", NormalizedName = "PROJECTMANAGER" },
                new IdentityRole { Id = "3", Name = "Developer", NormalizedName = "DEVELOPER" },
                new IdentityRole { Id = "4", Name = "Tester", NormalizedName = "TESTER" },
                new IdentityRole { Id = "5", Name = "Guest", NormalizedName = "GUEST" }
            );
        }
    }
}
