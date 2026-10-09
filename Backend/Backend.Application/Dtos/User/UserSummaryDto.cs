using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.User;

public class UserSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool IsActive { get; set; }
}