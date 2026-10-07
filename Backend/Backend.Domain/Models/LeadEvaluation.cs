using System;
using System.Collections.Generic;

namespace Backend.Domain.Models;

public partial class LeadEvaluation
{
    public int Id { get; set; }

    public int LeadId { get; set; }

    public DateTime EvaluatedAt { get; set; }

    public decimal CalculatedScore { get; set; }

    public string ConfidenceRating { get; set; } = null!;

    public string InputDataJson { get; set; } = null!;

    public string ScoreBreakdownJson { get; set; } = null!;

    public bool IsUserOverridden { get; set; }

    public decimal? OverriddenScore { get; set; }

    public string? OverrideReason { get; set; }

    public int EvaluatedByUserId { get; set; }

    public virtual User EvaluatedByUser { get; set; } = null!;

    public virtual Lead Lead { get; set; } = null!;
}
