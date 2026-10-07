using System;
using System.Collections.Generic;

namespace Backend.Domain.Models;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Industry { get; set; }

    public int? EmployeeCount { get; set; }

    public decimal? AnnualRevenue { get; set; }

    public string? Website { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
