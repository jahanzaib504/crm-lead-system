using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
public interface IUserManager
{
    int? GetCurrentUserId();
    string? GetCurrentUserEmail();
    string? GetCurrentUserRole();
    string? GetCurrentUserName();
    int? GetCurrentUserRoleId();
}

namespace Backend.Application.Services
{
    public class UserManager : IUserManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? user?.FindFirst("sub")?.Value
                          ?? user?.FindFirst("nameid")?.Value;

            if (int.TryParse(idClaim, out int id))
            {
                return id;
            }
            return null;
        }

        public string? GetCurrentUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Email)?.Value 
                ?? user?.FindFirst("email")?.Value;
        }

        public string? GetCurrentUserRole()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Role)?.Value 
                ?? user?.FindFirst("role")?.Value;
        }

        public string? GetCurrentUserName()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Name)?.Value 
                ?? user?.FindFirst("name")?.Value 
                ?? GetCurrentUserEmail();
        }
        public int? GetCurrentUserRoleId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var roleIdClaim = user?.FindFirst("role_id")?.Value;

            if (int.TryParse(roleIdClaim, out int roleId))
            {
                return roleId;
            }
            return null;
        }
    }
}
