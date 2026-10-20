using System;

namespace Backend.Application.Dtos.Lead;

public class UpdateLeadDto
{
    public string? Title { get; set; }
    public string? Query { get; set; }
    public string? Source { get; set; }
    public decimal? EstimatedValue { get; set; }
    public int? LeadStageId { get; set; }
}
