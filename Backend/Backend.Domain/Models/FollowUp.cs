using System;
using System.Collections.Generic;

namespace Backend.Domain.Models;

public partial class FollowUp
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public int AssignedToUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User AssignedToUser { get; set; } = null!;

    public virtual Lead Lead { get; set; } = null!;
}
