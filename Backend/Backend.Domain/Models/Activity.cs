using System;
using System.Collections.Generic;

namespace Backend.Domain.Models;

public partial class Activity
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public string ActivityType { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string? Summary { get; set; }

    public DateTime ActivityDate { get; set; }

    public int LoggedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Lead Lead { get; set; } = null!;

    public virtual User LoggedByUser { get; set; } = null!;
}
