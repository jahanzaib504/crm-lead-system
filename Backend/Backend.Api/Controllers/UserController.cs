using Backend.Application.Dtos.User;
using Backend.Application.Dtos.Api;
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
    public async Task<ActionResult<ApiResponse<bool>>> ApproveUser(int id, [FromBody] ApproveUserDto dto)
    {
        var result = await _userService.ApproveUser(id, dto);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "User approved successfully", result));
    }

    [HttpPost("grant-access/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> GrantAccessToUser(int id)
    {
        var result = await _userService.GrantAccessToUser(id);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "User access granted successfully", result));
    }

    [HttpPost("revoke-access/{id}")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<ApiResponse<bool>>> RevokeAccess(int id)
    {
        var result = await _userService.RevokeAccess(id);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "User access revoked successfully", result));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetMe()
    {
        var result = await _userService.GetMe();
        return Ok(new ApiResponse<UserResponseDto>(StatusCodes.Status200OK, "Current user retrieved successfully", result));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetAllUsers()
    {
        var result = await _userService.GetAllUsers();
        return Ok(new ApiResponse<IEnumerable<UserResponseDto>>(StatusCodes.Status200OK, "Users retrieved successfully", result));
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetPendingUsers()
    {
        var result = await _userService.GetPendingUsers();
        return Ok(new ApiResponse<IEnumerable<UserResponseDto>>(StatusCodes.Status200OK, "Pending users retrieved successfully", result));
    }

    [HttpGet("managers")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetManagers()
    {
        var result = await _userService.GetManagers();
        return Ok(new ApiResponse<IEnumerable<UserResponseDto>>(StatusCodes.Status200OK, "Managers retrieved successfully", result));
    }

    [HttpGet("reps")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetSalesReps()
    {
        var result = await _userService.GetSalesReps();
        return Ok(new ApiResponse<IEnumerable<UserResponseDto>>(StatusCodes.Status200OK, "Sales representatives retrieved successfully", result));
    }
}
