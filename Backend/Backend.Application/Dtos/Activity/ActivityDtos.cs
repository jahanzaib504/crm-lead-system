using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.Dtos.Activity;

public class ActivityResponseDto
{
    public int Id { get; set; }
    public int LeadId { get; set; }
    public string ActivityType { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string? Summary { get; set; }
    public DateTime ActivityDate { get; set; }
    public int LoggedByUserId { get; set; }
    public string? LoggedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateActivityDto
{
    [Required]
    public int LeadId { get; set; }

    [Required, RegularExpression("^(Call|Meeting|Email|Demo)$", ErrorMessage = "ActivityType must be Call, Meeting, Email, or Demo")]
    public string ActivityType { get; set; } = "Call";

    [Required, MaxLength(200)]
    public string Subject { get; set; } = null!;

    public string? Summary { get; set; }

    public DateTime? ActivityDate { get; set; }
}
