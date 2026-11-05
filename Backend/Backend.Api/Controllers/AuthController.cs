

using Backend.Application.Dtos;
using Backend.Application.Dtos.Api;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RegisterUser([FromBody] RegisterUserDto userData)
    {
        var res = await _authService.RegisterUserAsync(userData);
        var response = new ApiResponse<AuthResponseDto>(StatusCodes.Status201Created, "User registered successfully", res);
        return StatusCode(StatusCodes.Status201Created, response);
    }
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> LoginUser([FromBody] LoginUserDto userData)
    {
        var res = await _authService.LoginUserAsync(userData);
        return Ok(new ApiResponse<AuthResponseDto>(StatusCodes.Status200OK, "User logged in successfully", res));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> LogoutUser(CancellationToken cancellationToken)
    {
        await _authService.LogoutUserAsync(cancellationToken);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "User logged out successfully", true));
    }
}
