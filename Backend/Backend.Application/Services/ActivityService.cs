using Backend.Application.Dtos.Activity;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Services;

public interface IActivityService
{
    Task<IEnumerable<ActivityResponseDto>> GetByLeadId(int leadId);
    Task<IEnumerable<ActivityResponseDto>> GetMyActivities();
    Task<ActivityResponseDto> CreateActivity(CreateActivityDto dto);
    Task<bool> DeleteActivity(int id);
}

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepo;
    private readonly ILeadRepository _leadRepo;
    private readonly IUserManager _userManager;

    public ActivityService(
        IActivityRepository activityRepo,
        ILeadRepository leadRepo,
        IUserManager userManager)
    {
        _activityRepo = activityRepo ?? throw new ArgumentNullException(nameof(activityRepo));
        _leadRepo = leadRepo ?? throw new ArgumentNullException(nameof(leadRepo));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    private static ActivityResponseDto MapToDto(Activity a)
    {
        return new ActivityResponseDto
        {
            Id = a.Id,
            LeadId = a.LeadId,
            ActivityType = a.ActivityType,
            Subject = a.Subject,
            Summary = a.Summary,
            ActivityDate = a.ActivityDate,
            LoggedByUserId = a.LoggedByUserId,
            LoggedByUserName = a.LoggedByUser?.FullName,
            CreatedAt = a.CreatedAt
        };
    }

    public async Task<IEnumerable<ActivityResponseDto>> GetByLeadId(int leadId)
    {
        var lead = await _leadRepo.GetByIdAsync(leadId);
        if (lead == null) throw new NotFoundException("Lead not found.");

        var activities = await _activityRepo.GetByLeadIdAsync(leadId);
        return activities.Select(MapToDto);
    }

    public async Task<IEnumerable<ActivityResponseDto>> GetMyActivities()
    {
        var currentUserId = _userManager.GetCurrentUserId();
        if (!currentUserId.HasValue) throw new UnauthorizedException("User not authenticated.");

        var currentRole = _userManager.GetCurrentUserRole();
        if (currentRole == "Admin")
        {
            var all = await _activityRepo.GetAllAsync();
            return all.OrderByDescending(a => a.ActivityDate).Select(MapToDto);
        }

        var list = await _activityRepo.GetByUserIdAsync(currentUserId.Value);
        return list.Select(MapToDto);
    }

    public async Task<ActivityResponseDto> CreateActivity(CreateActivityDto dto)
    {
        var lead = await _leadRepo.GetByIdAsync(dto.LeadId);
        if (lead == null) throw new NotFoundException("Lead not found.");

        var currentUserId = _userManager.GetCurrentUserId();
        if (!currentUserId.HasValue) throw new UnauthorizedException("User not authenticated.");

        var activity = new Activity
        {
            LeadId = dto.LeadId,
            ActivityType = dto.ActivityType,
            Subject = dto.Subject,
            Summary = dto.Summary,
            ActivityDate = dto.ActivityDate ?? DateTime.UtcNow,
            LoggedByUserId = currentUserId.Value,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userManager.GetCurrentUserName() ?? _userManager.GetCurrentUserEmail() ?? "User",
            IsActive = true
        };

        var created = await _activityRepo.AddAsync(activity);
        var fetched = await _activityRepo.GetByIdAsync(created.Id);
        return MapToDto(fetched ?? created);
    }

    public async Task<bool> DeleteActivity(int id)
    {
        return await _activityRepo.DeleteAsync(id);
    }
}
