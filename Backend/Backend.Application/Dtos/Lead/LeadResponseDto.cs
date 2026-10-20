using Backend.Application.Dtos.LeadStage;
using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Application.Dtos.Lead;

public class LeadResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Query { get; set; } = null!;

    public int ContactId { get; set; }

    public int? CompanyId { get; set; }

    public int LeadStageId { get; set; }

    public int? AssignedToUserId { get; set; }

    public string Source { get; set; } = null!;

    public decimal? EstimatedValue { get; set; }

    public decimal? CurrentScore { get; set; }

    public int? ManagerId { get; set; }

    public string? ManagerName { get; set; }

    public string? AssignedToUserName { get; set; }

    public string? ContactName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactJobTitle { get; set; }

    public string? CompanyName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public LeadStageDto? LeadStage { get; set; } = null!;
}
