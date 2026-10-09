

using Backend.Application.Dtos;
using Backend.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Backend.Application.Dtos.User;
namespace Backend.Api.Controllers;

[ApiController] 
[Route("auth")]
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
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> LoginUser([FromBody] LoginUserDto userData) // 3. Use async/await and ActionResult
    {
        // 4. RegisterUserAsync returns a Task, so you MUST await it
        var res = await _authService.LoginUserAsync(userData);
        return Ok(res);
    }
}
