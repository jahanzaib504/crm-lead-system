using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Application.Dtos.AuditLog;

public class AuditLogRequestDto
{
    [Required, Range(1, int.MaxValue)]
    public int page { get; set; }
    [Required, Range(1, int.MaxValue)]
    public int pageSize { get; set; }
    public string? search { get; set; } = null!;

}
