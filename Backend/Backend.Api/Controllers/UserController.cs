using Backend.Application.Services;
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
    [Authorize(Roles = "Sales Manager, Admin")]
    public async Task<bool> GrantAccessToUser(int id)
    {
        return await _userService.GrantAccessToUser(id);
    }
    [HttpPost("revoke-access/{id}")]
    [Authorize(Roles = "Sales Manager, Admin")]
    public async Task<bool> RevokeAccess(int id)
    {
        return await _userService.RevokeAccess(id);
    }
}
