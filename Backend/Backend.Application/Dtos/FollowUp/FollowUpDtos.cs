using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.Dtos.FollowUp;

public class FollowUpResponseDto
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public string Title { get; set; } = null!;
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public int AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
}

public class CreateFollowUpDto
{
    [Required]
    public int LeadId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    public DateTime DueDate { get; set; }

    [MaxLength(20)]
    public string Priority { get; set; } = "Medium";
}

public class UpdateFollowUpStatusDto
{
    [Required]
    public string Status { get; set; } = "Completed";
}
