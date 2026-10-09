using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.Contact;

public class CreateContactDto
{
    public int? CompanyId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? JobTitle { get; set; }
}
