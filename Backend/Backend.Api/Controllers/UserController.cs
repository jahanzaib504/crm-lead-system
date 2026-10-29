using Backend.Application.Dtos.User;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("approve/{id}")] // This endpoint is used to approve a user by their ID. It only works for first sign up
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> ApproveUser(int id, [FromBody] ApproveUserDto dto)
    {
        var result = await _userService.ApproveUser(id, dto);
        return Ok(result);
    }

    [HttpPost("grant-access/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> GrantAccessToUser(int id)
    {
        var result = await _userService.GrantAccessToUser(id);
        return Ok(result);
    }

    [HttpPost("revoke-access/{id}")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<bool>> RevokeAccess(int id)
    {
        var result = await _userService.RevokeAccess(id);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetMe()
    {
        var result = await _userService.GetMe();
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
    {
        var result = await _userService.GetAllUsers();
        return Ok(result);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetPendingUsers()
    {
        var result = await _userService.GetPendingUsers();
        return Ok(result);
    }

    [HttpGet("managers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetManagers()
    {
        var result = await _userService.GetManagers();
        return Ok(result);
    }

    [HttpGet("reps")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetSalesReps()
    {
        var result = await _userService.GetSalesReps();
        return Ok(result);
    }
}
