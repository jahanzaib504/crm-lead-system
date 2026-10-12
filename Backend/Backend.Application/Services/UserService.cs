using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Backend.Application.Services;

public interface IUserService
{
    Task<bool> GrantAccessToUser(int id);
    Task<bool> RevokeAccess(int id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IUserManager _userManager;

    public UserService(IUserRepository userRepo, IUserManager userManager)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<bool> GrantAccessToUser(int id)
    {
        var currentUserId = _userManager.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedException("Current user is not authenticated.");
        }

        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User does not exist");
        }

        if (user.ManagerId.HasValue)
        {
            throw new ConflictException("User is already assigned to a manager");
        }

        user.ManagerId = currentUserId.Value;
        user.IsActive = true;

        await _userRepo.UpdateAsync(user);
        return true;
    }

    public async Task<bool> RevokeAccess(int id)
    {
        var currentUserId = _userManager.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedException("Current user is not authenticated.");
        }

        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User does not exist");
        }

        // Verify that the manager revoking access is indeed the assigned manager
        if (user.ManagerId != currentUserId.Value)
        {
            throw new UnauthorizedException("You are not authorized to revoke access for this user.");
        }

        user.ManagerId = null;
        user.IsActive = false;

        await _userRepo.UpdateAsync(user);
        return true;
    }
}