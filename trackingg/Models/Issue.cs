using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using trackingg.Data;

namespace trackingg.Models
{
    // Existing Issue model with some enhancements
    public class Issue
    {
        public uint Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;
        public IssueType IssueType { get; set; }
        public Priority Priority { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        
        // Foreign Keys
        [Required]
        public string AssignedToId { get; set; } = string.Empty;
        public ApplicationUser AssignedTo { get; set; } = null!;
        
        public uint? ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }

    public class Project
    {
        public uint Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }
        
        // Foreign Keys
        [Required]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;
        
        // Navigation properties
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }

    public class ProjectMember
    {
        public uint Id { get; set; }
        
        // Foreign Keys
        public uint ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        
        public ProjectRole Role { get; set; }
        public DateTime JoinedDate { get; set; }
    }

    public class Comment
    {
        public uint Id { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
        
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        
        // Foreign Keys
        public uint IssueId { get; set; }
        public Issue Issue { get; set; } = null!;
        
        [Required]
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser Author { get; set; } = null!;
    }

    public class Tag
    {
        public uint Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(7)]
        public string Color { get; set; } = "#007bff"; // Default color
        
        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
    }

    public class Activity
    {
        public uint Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        public ActivityType Type { get; set; }
        public DateTime Timestamp { get; set; }
        
        // Foreign Keys
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        
        public uint? IssueId { get; set; }
        public Issue Issue { get; set; } = null!;
        
        public uint? ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    }

    public enum ProjectRole
    {
        Viewer,
        Member,
        Manager,
        Owner
    }

    public enum ActivityType
    {
        IssueCreated,
        IssueUpdated,
        IssueResolved,
        CommentAdded,
        ProjectCreated,
        ProjectUpdated,
        MemberAdded,
        MemberRemoved
    }
}