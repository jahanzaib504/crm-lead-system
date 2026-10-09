using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Dtos.User;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserResponseDto User { get; set; } = null!;
}