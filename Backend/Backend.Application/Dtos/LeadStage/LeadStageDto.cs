using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.LeadStage;

public class LeadStageDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public decimal ConversionWeight { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
