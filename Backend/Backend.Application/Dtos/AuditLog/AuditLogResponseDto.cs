using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.AuditLog;

public class AuditLogResponseDto
{
    public int Id { get; set; }

    public string EntityName { get; set; } = null!;

    public int EntityId { get; set; }

    public string Action { get; set; } = null!;

    public string? ChangesJson { get; set; }

    public int PerformedByUserId { get; set; }

    public DateTime Timestamp { get; set; }

}
