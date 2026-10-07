using System;
using System.Collections.Generic;

namespace Backend.Domain.Models;

public partial class Contact
{
    public int Id { get; set; }

    public int? CompanyId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? JobTitle { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Company? Company { get; set; }

    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}
