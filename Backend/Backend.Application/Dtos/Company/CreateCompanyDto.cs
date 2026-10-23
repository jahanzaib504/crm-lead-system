using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Application.Dtos.Company;

public class CreateCompanyDto
{
    [Required(ErrorMessage = "Company name is required.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Industry name is required.")]
    [StringLength(100, ErrorMessage = "Industry name is too long.")]
    public string Industry { get; set; } = null!;

    [Required(ErrorMessage = "Employee count is required.")]
    public int EmployeeCount { get; set; }

    [Required(ErrorMessage = "Annual revenue is required.")]
    public decimal AnnualRevenue { get; set; }

    [Required(ErrorMessage = "Website is required.")]
    public string Website { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    public string Phone { get; set; } = null!;
}
