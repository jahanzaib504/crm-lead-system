using System;
using System.Collections.Generic;

namespace Backend.Application.Dtos.Analytics;

public class StageCountDto
{
    public string StageName { get; set; } = null!;
    public int Count { get; set; }
}

public class SourceCountDto
{
    public string Source { get; set; } = null!;
    public int Count { get; set; }
}

public class RepPerformanceDto
{
    public int RepId { get; set; }
    public string RepName { get; set; } = null!;
    public int LeadCount { get; set; }
    public decimal TotalValue { get; set; }
}

public class AnalyticsResponseDto
{
    public string Role { get; set; } = null!;

    // Common / Admin Metrics
    public int TotalLeads { get; set; }
    public int OpenPoolLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public decimal TotalPipelineValue { get; set; }
    public double ConversionRate { get; set; }
    public int TotalUsers { get; set; }
    public int PendingApprovals { get; set; }

    // Manager Metrics
    public int TeamLeadsCount { get; set; }
    public int ActiveRepsCount { get; set; }
    public decimal TeamPipelineValue { get; set; }
    public List<RepPerformanceDto> RepPerformance { get; set; } = new();

    // Rep Metrics
    public int MyLeadsCount { get; set; }
    public decimal MyPipelineValue { get; set; }
    public int PendingFollowUpsCount { get; set; }
    public int CompletedActivitiesCount { get; set; }

    // Breakdowns
    public List<StageCountDto> StageBreakdown { get; set; } = new();
    public List<SourceCountDto> SourceBreakdown { get; set; } = new();
}
