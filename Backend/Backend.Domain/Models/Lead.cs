using System;
using System.Collections.Generic;

namespace Backend.Domain.Models;

public partial class Lead
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int ContactId { get; set; }

    public int? CompanyId { get; set; }

    public int LeadStageId { get; set; }

    public int? AssignedToUserId { get; set; }

    public string Source { get; set; } = null!;

    public decimal? EstimatedValue { get; set; }

    public decimal? CurrentScore { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual User? AssignedToUser { get; set; }

    public virtual Company? Company { get; set; }

    public virtual Contact Contact { get; set; } = null!;

    public virtual ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();

    public virtual ICollection<LeadEvaluation> LeadEvaluations { get; set; } = new List<LeadEvaluation>();

    public virtual LeadStage LeadStage { get; set; } = null!;

    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}
