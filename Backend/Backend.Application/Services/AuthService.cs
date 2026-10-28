using Backend.Application.Dtos.User;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;


namespace Backend.Application.Services;
public class AuthService
{
    private readonly IUserRepository _userRepo;
    private readonly TokenService _tokenService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuditLogService _auditLogService;
    private readonly IUserManager _userManager;
    private UserResponseDto MapToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            RoleId = user.RoleId,

            Role = new RoleDto { Id = user.Role.Id, Name = user.Role.Name },

            ManagerId = user.ManagerId,
            ManagerName = user.Manager?.FullName,

            IsActive = user.IsActive,

            CreatedAt = user.CreatedAt,
            CreatedBy = user.CreatedBy,

            UpdatedAt = user.UpdatedAt,
            UpdatedBy = user.UpdatedBy
        };
    }
    public AuthService(
        IUserRepository userRepo,
        TokenService tokenService,
        IPasswordHasher<User> passwordHasher,
        AuditLogService auditLogService,
        IUserManager userManager)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _auditLogService = auditLogService;
        _userManager = userManager;
    }
    public async Task<AuthResponseDto> RegisterUserAsync(RegisterUserDto userData)
    {
        User? user = await _userRepo.GetByEmailAsync(userData.Email);
        if (user != null)
        {
            if (!user.IsActive)
            {
                throw new UnauthorizedException("An account with this email is already registered and pending administrator approval.");
            }
            throw new ConflictException("User with this email already exists");
        }

        int roleId = userData.RoleId ?? 3; // Default to Sales Rep (role 3)

        User userToBeStored = new User
        {
            FullName = userData.FullName,
            Email = userData.Email,
            RoleId = roleId,
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SelfRegistration"
        };

        var hashedPassword = _passwordHasher.HashPassword(userToBeStored, userData.Password);
        userToBeStored.PasswordHash = hashedPassword;

        var createdUser = await _userRepo.CreateAsync(userToBeStored);
        await _auditLogService.LogAsync(
            "User",
            createdUser.Id,
            "create",
            new { createdUser.FullName, createdUser.Email, createdUser.RoleId },
            createdUser.Id);

        // Do not issue JWT token for inactive users; they must wait for admin approval
        return new AuthResponseDto
        {
            Token = string.Empty,
            User = MapToDto(createdUser)
        };
    }

    public async Task<AuthResponseDto> LoginUserAsync(LoginUserDto userData)
    {
        var user = await _userRepo.GetByEmailAsync(userData.Email);

        bool isPasswordMatch = user != null && _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userData.Password) == PasswordVerificationResult.Success;

        if (user == null || !isPasswordMatch) 
            throw new NotFoundException("Incorrect email or password");

        if (!user.IsActive)
        {
            throw new UnauthorizedException("Your account is pending administrator approval. Please wait for an administrator to activate your account.");
        }

        var (token, expiresAt) = _tokenService.CreateToken(user);
        await _auditLogService.LogAsync(
            "User",
            user.Id,
            "login",
            new { user.Email },
            user.Id);

        return new AuthResponseDto
        {
            Token = token,
            User = MapToDto(user),
            ExpiresAt = expiresAt
        };
    }

    public async Task LogoutUserAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userManager.GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new UnauthorizedException("Not authenticated.");
        }

        var email = _userManager.GetCurrentUserEmail();
        await _auditLogService.LogAsync(
            "User",
            userId.Value,
            "logout",
            new { Email = email },
            userId.Value,
            cancellationToken);
    }
}

