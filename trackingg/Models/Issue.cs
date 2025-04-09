using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using trackingg.Data;

namespace trackingg.Models
{
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

        public string AssignedToId { get; set; } = string.Empty;

        [ValidateNever]
        public ApplicationUser AssignedTo { get; set; }

        public uint? ProjectId { get; set; }

        [ValidateNever]
        public Project Project { get; set; }

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

        [Required]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; }

        public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();
        public virtual ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }

    public class ProjectMember
    {
        public uint Id { get; set; }

        public uint ProjectId { get; set; }
        public Project Project { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }

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

        public uint IssueId { get; set; }
        public Issue Issue { get; set; }

        [Required]
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser Author { get; set; }
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

        public ActivityType Type { get; set; }
        public DateTime Timestamp { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; }

        public uint? IssueId { get; set; }
        public Issue Issue { get; set; }

        public uint? ProjectId { get; set; }
        public Project Project { get; set; }
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
