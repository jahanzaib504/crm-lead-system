using System;
using System.Collections.Generic;

namespace Backend.Domain.Models;

public partial class AuditLog
{
    public int Id { get; set; }

    public string EntityName { get; set; } = null!;

    public int EntityId { get; set; }

    public string Action { get; set; } = null!;

    public string? ChangesJson { get; set; }

    public int PerformedByUserId { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual User PerformedByUser { get; set; } = null!;
}
