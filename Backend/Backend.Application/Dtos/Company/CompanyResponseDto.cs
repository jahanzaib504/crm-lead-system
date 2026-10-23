using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.Company;

public class CompanyResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Industry { get; set; }

    public int? EmployeeCount { get; set; }

    public decimal? AnnualRevenue { get; set; }

    public string? Website { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

}
