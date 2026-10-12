using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
public interface IUserManager
{
    int? GetCurrentUserId();
    public string? GetCurrentUserEmail();
    public string? GetCurrentUserRole();

}
namespace Backend.Application.Services
{
    public class UserManager: IUserManager
    {
        IHttpContextAccessor _httpContextAccessor;
        public UserManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return int.Parse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        public string? GetCurrentUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Email)?.Value;
        }
        public string? GetCurrentUserRole()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Role)?.Value;
        }

    }
}
