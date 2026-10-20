using System.ComponentModel.DataAnnotations;

namespace Backend.Application.Dtos.User;

public class ApproveUserDto
{
    [Required]
    [Range(1, 3)]
    public int RoleId { get; set; }

    public int? ManagerId { get; set; }

    public bool IsActive { get; set; } = true;
}
