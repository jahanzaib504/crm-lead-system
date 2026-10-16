using Backend.Application.Dtos.User;
using Backend.Application.Services;
using Backend.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost("grant-access/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<bool> GrantAccessToUser(int id)
    {
        return await _userService.GrantAccessToUser(id);
    }
    [HttpPost("revoke-access/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<bool> RevokeAccess(int id)
    {
        return await _userService.RevokeAccess(id);
    }
    [HttpGet("me")]
    [Authorize]
    public async Task<UserResponseDto> GetMe()
    {
        return await _userService.GetMe();
    }
    [HttpGet]
    [Authorize(Roles="Admin, Sales Manager")]
    public async Task<IEnumerable<UserResponseDto>> GetAllUsers() // Retreives all users working under this person
    {
        return await _userService.GetAllUsers();
    }
}
