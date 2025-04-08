// --- START OF FILE ---

// Required using statements
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // Needed for [ValidateNever]
using trackingg.Data; // Your data namespace - adjust if needed

namespace trackingg.Models // Your models namespace - adjust if needed
{
    // --- Issue Class (Final Modifications) ---
    public class Issue
    {
        public uint Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Description { get; set; } = string.Empty;

        public IssueType IssueType { get; set; } // Refers to IssueType enum defined elsewhere
        public Priority Priority { get; set; } // Refers to Priority enum defined elsewhere

        public DateTime Created { get; set; }
        public DateTime? Completed { get; set; }

        // --- Foreign Keys ---

        // [Required] // <-- MODIFIED: Removed [Required]
        public string AssignedToId { get; set; } = string.Empty;

        [ValidateNever] // <-- MODIFIED: Added [ValidateNever]
        public ApplicationUser AssignedTo { get; set; } // <-- MODIFIED: Removed = null!;


        public uint? ProjectId { get; set; } // Nullable Foreign Key

        [ValidateNever] // <-- MODIFIED: Added [ValidateNever] (from previous step)
        public Project Project { get; set; } // <-- MODIFIED: Removed = null!;


        // --- Collections ---
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }

    // --- Other Model Classes (with null! removed from nav props) ---
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

        [Required]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } // <-- MODIFIED: Removed = null!;

        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }

    public class ProjectMember
    {
        public uint Id { get; set; }

        public uint ProjectId { get; set; }
        public Project Project { get; set; } // <-- MODIFIED: Removed = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } // <-- MODIFIED: Removed = null!;

        public ProjectRole Role { get; set; } // Refers to ProjectRole enum below
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

        public uint IssueId { get; set; }
        public Issue Issue { get; set; } // <-- MODIFIED: Removed = null!;

        [Required]
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser Author { get; set; } // <-- MODIFIED: Removed = null!;
    }

    public class Tag
    {
        public uint Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(7)]
        public string Color { get; set; } = "#007bff";

        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
    }

    public class Activity
    {
        public uint Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public ActivityType Type { get; set; } // Refers to ActivityType enum below
        public DateTime Timestamp { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } // <-- MODIFIED: Removed = null!;

        public uint? IssueId { get; set; }
        public Issue Issue { get; set; } // <-- MODIFIED: Removed = null!;

        public uint? ProjectId { get; set; }
        public Project Project { get; set; } 
    }

    // --- Enums used by the classes above ---
    // --- Assuming IssueType and Priority are in separate files now ---
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

} // --- End of namespace trackingg.Models ---

// --- END OF FILE ---