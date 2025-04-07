using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using trackingg.Models;

namespace trackingg.Data
{
    // ApplicationUser class to extend Identity User
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }

        // Navigation properties
        public virtual ICollection<Issue> AssignedIssues { get; set; } = new List<Issue>();
        public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
        public virtual ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }

    // Updated DbContext to include Identity
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Issue> Issues { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Activity> Activities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure many-to-many relationship between Issue and Tag
            builder.Entity<Issue>()
                .HasMany(i => i.Tags)
                .WithMany(t => t.Issues)
                .UsingEntity(j => j.ToTable("IssueTags"));

            // Configure foreign key relationship between Issue and ApplicationUser (AssignedTo)
            builder.Entity<Issue>()
                .HasOne(i => i.AssignedTo)
                .WithMany(u => u.AssignedIssues)
                .HasForeignKey(i => i.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust DeleteBehavior as needed

            // Configure foreign key relationship between Issue and Project
            builder.Entity<Issue>()
                .HasOne(i => i.Project)
                .WithMany(p => p.Issues)
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust DeleteBehavior as needed

            // Seed Roles
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