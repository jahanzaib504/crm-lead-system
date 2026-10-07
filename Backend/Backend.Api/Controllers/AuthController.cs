

using Backend.Application.Dtos;
using Backend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController] 
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")] 
    public async Task<ActionResult<UserResponseDto>> RegisterUser([FromBody] RegisterUserDto userData) // 3. Use async/await and ActionResult
    {
        // 4. RegisterUserAsync returns a Task, so you MUST await it
        var user = await _authService.RegisterUserAsync(userData);
        return Ok(user);
    }
}
