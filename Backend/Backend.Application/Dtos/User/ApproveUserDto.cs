using System.ComponentModel.DataAnnotations;

namespace Backend.Application.Dtos.User;

public class ApproveUserDto
{
    [Required]
    [Range(2, 3, ErrorMessage = "RoleId must be either 2 (Sales Manager) or 3 (Sales Rep).")]
    public int RoleId { get; set; }

    public int? ManagerId { get; set; }

    public bool IsActive { get; set; } = true;
}
