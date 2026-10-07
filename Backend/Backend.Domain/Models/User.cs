using System;
using System.Collections.Generic;

namespace Backend.Domain.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? ManagerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();

    public virtual ICollection<User> InverseManager { get; set; } = new List<User>();

    public virtual ICollection<LeadEvaluation> LeadEvaluations { get; set; } = new List<LeadEvaluation>();

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();

    public virtual User? Manager { get; set; }
}
