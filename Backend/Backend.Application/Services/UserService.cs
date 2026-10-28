using Backend.Application.Dtos.User;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Services;

public interface IUserService
{
    Task<bool> ApproveUser(int id, ApproveUserDto dto);
    Task<bool> UpdateUserAccess(int id, ApproveUserDto dto);
    Task<bool> GrantAccessToUser(int id);
    Task<bool> RevokeAccess(int id);
    Task<UserResponseDto> GetMe();
    Task<IEnumerable<UserResponseDto>> GetAllUsers();
    Task<IEnumerable<UserResponseDto>> GetPendingUsers();
    Task<IEnumerable<UserResponseDto>> GetManagers();
    Task<IEnumerable<UserResponseDto>> GetSalesReps();
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IUserManager _userManager;
    private readonly AuditLogService _auditLogService;

    private UserResponseDto MapToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            RoleId = user.RoleId,

            Role = new RoleDto { Id =  user.Role.Id , Name = user.Role.Name},

            ManagerId = user.ManagerId,
            ManagerName = user.Manager?.FullName,

            IsActive = user.IsActive,

            CreatedAt = user.CreatedAt,
            CreatedBy = user.CreatedBy,

            UpdatedAt = user.UpdatedAt,
            UpdatedBy = user.UpdatedBy
        };
    }
    
    public UserService(
        IUserRepository userRepo,
        IUserManager userManager,
        AuditLogService auditLogService)
    {
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
    }


    public async Task<bool> ApproveUser(int id, ApproveUserDto dto)
    {
        var currentRole = _userManager.GetCurrentUserRole();
        if (currentRole != "Admin")
        {
            throw new UnauthorizedException("Only administrators can approve users.");
        }

        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User does not exist.");
        }

        // Validate manager if provided
        if (dto.ManagerId.HasValue)
        {
            var manager = await _userRepo.GetByIdAsync(dto.ManagerId.Value);
            if (manager == null)
            {
                throw new NotFoundException("Selected manager does not exist.");
            }
        }

        user.RoleId = dto.RoleId;

        if (dto.RoleId == 2) // Admin is manager of sales manager
            user.ManagerId = _userManager.GetCurrentUserId() ?? null;
        else if (dto.RoleId == 3 && dto.ManagerId.HasValue) // 
            user.ManagerId = dto.ManagerId;
        else if (dto.RoleId == 3) // No manager provided
            throw new ConflictException("Sales Repceptionists must have a manager");
        else
            user.ManagerId = null; // If the user is admin than he cannot have a manager

        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = _userManager.GetCurrentUserEmail() ?? "Admin";

        var updatedUser = await _userRepo.UpdateAsync(user);
        await _auditLogService.LogAsync(
            "User",
            updatedUser.Id,
            "update",
            dto,
            _userManager.GetCurrentUserId());
        return true;
    }

    public async Task<bool> UpdateUserAccess(int id, ApproveUserDto dto)
    {
        return await ApproveUser(id, dto);
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

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = _userManager.GetCurrentUserEmail() ?? "Admin";

        var updatedUser = await _userRepo.UpdateAsync(user);
        await _auditLogService.LogAsync(
            "User",
            updatedUser.Id,
            "update",
            new { updatedUser.IsActive },
            currentUserId);
        return true;
    }

    public async Task<bool> RevokeAccess(int id)
    {
        var currentUserId = _userManager.GetCurrentUserId();
        var currentRole = _userManager.GetCurrentUserRole();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedException("Current user is not authenticated.");
        }

        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User does not exist");
        }

        // Admin can revoke anyone; Manager can only revoke their own reps
        if (currentRole != "Admin" && user.ManagerId != currentUserId.Value)
        {
            throw new UnauthorizedException("You are not authorized to revoke access for this user.");
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = _userManager.GetCurrentUserEmail() ?? "Admin";

        var updatedUser = await _userRepo.UpdateAsync(user);
        await _auditLogService.LogAsync(
            "User",
            updatedUser.Id,
            "update",
            new { updatedUser.IsActive },
            currentUserId);
        return true;
    }

    public async Task<UserResponseDto> GetMe()
    {
        int? userId = _userManager.GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new UnauthorizedException("Not authenticated.");
        }

        var user = await _userRepo.GetByIdAsync(userId.Value);
        if (user == null)
        {
            throw new NotFoundException("User not found.");
        }

        return MapToDto(user);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsers()
    {
        var users = await _userRepo.GetAllAsync();
        var currentUserRole = _userManager.GetCurrentUserRole();
        var currentUserId = _userManager.GetCurrentUserId();

        if (currentUserRole == "Admin")
        {
            return users.Select(MapToDto);
        }

        if (currentUserRole == "Sales Manager")
        {
            return users.Where(u => u.ManagerId == currentUserId).Select(MapToDto);
        }

        return Enumerable.Empty<UserResponseDto>();
    }

    public async Task<IEnumerable<UserResponseDto>> GetPendingUsers()
    {
        var users = await _userRepo.GetAllAsync();
        return users.Where(u => !u.IsActive).Select(MapToDto);
    }

    public async Task<IEnumerable<UserResponseDto>> GetManagers()
    {
        var users = await _userRepo.GetAllAsync();
        // Return active Sales Managers (role 2)
        return users.Where(u => u.IsActive && u.RoleId == 2).Select(MapToDto);
    }

    public async Task<IEnumerable<UserResponseDto>> GetSalesReps()
    {
        var users = await _userRepo.GetAllAsync();
        var currentRole = _userManager.GetCurrentUserRole();
        var currentUserId = _userManager.GetCurrentUserId();

        if (string.Equals(currentRole, "Sales Manager", StringComparison.OrdinalIgnoreCase))
        {
            return users.Where(u => u.IsActive && u.RoleId == 3 && u.ManagerId == currentUserId).Select(MapToDto);
        }

        // Admin can see all active sales reps
        return users.Where(u => u.IsActive && u.RoleId == 3).Select(MapToDto);
    }
}