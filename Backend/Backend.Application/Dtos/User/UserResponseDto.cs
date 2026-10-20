using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.User;
public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
public class UserResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
    public RoleDto Role { get; set; } = null!;
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
