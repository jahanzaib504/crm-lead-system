using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Backend.Application.Dtos.User;

public class UpdateUserDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    public string Role { get; set; } = null!;

    public int? ManagerId { get; set; }

    public bool IsActive { get; set; }
}