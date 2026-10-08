using Backend.Application.Dtos;
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
    public AuthService(IUserRepository userRepo, TokenService tokenService, IPasswordHasher<User> passwordHasher)
    {
        _userRepo = userRepo;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }
    public async Task<UserResponseDto> RegisterUserAsync(RegisterUserDto userData)
    {
        User? user = await _userRepo.GetByEmailAsync(userData.Email);
        bool userExists = user != null;
        // User exists but is not granted access

        if (userExists && !user.IsActive)
        {
            throw new UnauthorizedException("User access is pending");
        }

        if (userExists)
        {
            throw new ConflictException("User already exists");
        }




        User userToBeStored = new User { FullName = userData.FullName, Email = userData.Email, Role = userData.Role };


        // Hash password

        var hashedPassword = _passwordHasher.HashPassword(userToBeStored, userData.Password);
        userToBeStored.PasswordHash = hashedPassword;

        await _userRepo.CreateAsync(userToBeStored);

        // Skipping email verification
        var accessToken = _tokenService.CreateToken(userToBeStored);
        return userToBeStored.Adapt<UserResponseDto>();

    }

    public async Task<AuthResponseDto> LoginUserAsync(LoginUserDto userData)
    {
        var user = await _userRepo.GetByEmailAsync(userData.Email);

        bool isPasswordMatch = user != null && _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userData.Password) == PasswordVerificationResult.Success;

        if (user == null || !isPasswordMatch) 
            throw new NotFoundException("Incorrect email or password");

            // Assign a authorization token
        var token = _tokenService.CreateToken(user);

        var tokenResponse = new AuthResponseDto { Token = token };

        return tokenResponse;

    }

    }



