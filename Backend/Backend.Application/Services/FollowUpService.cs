using Backend.Application.Dtos.FollowUp;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Services;

public interface IFollowUpService
{
    Task<IEnumerable<FollowUpResponseDto>> GetByLeadId(int leadId);
    Task<IEnumerable<FollowUpResponseDto>> GetMyPendingFollowUps();
    Task<FollowUpResponseDto> CreateFollowUp(CreateFollowUpDto dto);
    Task<bool> UpdateStatus(int id, string status);
    Task<bool> DeleteFollowUp(int id);
}

public class FollowUpService : IFollowUpService
{
    private readonly IFollowUpRepository _followUpRepo;
    private readonly ILeadRepository _leadRepo;
    private readonly IUserManager _userManager;
    private readonly AuditLogService _auditLogService;

    public FollowUpService(
        IFollowUpRepository followUpRepo,
        ILeadRepository leadRepo,
        IUserManager userManager,
        AuditLogService auditLogService)
    {
        _followUpRepo = followUpRepo ?? throw new ArgumentNullException(nameof(followUpRepo));
        _leadRepo = leadRepo ?? throw new ArgumentNullException(nameof(leadRepo));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
    }

    private static FollowUpResponseDto MapToDto(FollowUp f)
    {
        return new FollowUpResponseDto
        {
            Id = f.Id,
            LeadId = f.LeadId,
            Title = f.Title,
            DueDate = f.DueDate,
            Status = f.Status,
            Priority = f.Priority,
            AssignedToUserId = f.AssignedToUserId,
            AssignedToUserName = f.AssignedToUser?.FullName,
            CreatedAt = f.CreatedAt,
            CreatedBy = f.CreatedBy
        };
    }

    public async Task<IEnumerable<FollowUpResponseDto>> GetByLeadId(int leadId)
    {
        var lead = await _leadRepo.GetByIdAsync(leadId);
        if (lead == null) throw new NotFoundException("Lead not found.");

        var followUps = await _followUpRepo.GetByLeadIdAsync(leadId);
        return followUps.Select(MapToDto);
    }

    public async Task<IEnumerable<FollowUpResponseDto>> GetMyPendingFollowUps()
    {
        var currentUserId = _userManager.GetCurrentUserId();
        if (!currentUserId.HasValue) throw new UnauthorizedException("User not authenticated.");

        var currentRole = _userManager.GetCurrentUserRole();
        if (currentRole == "Admin")
        {
            var all = await _followUpRepo.GetAllAsync();
            return all.Where(f => f.Status == "Pending").OrderBy(f => f.DueDate).Select(MapToDto);
        }

        var followUps = await _followUpRepo.GetByUserIdAsync(currentUserId.Value);
        return followUps.Where(f => f.Status == "Pending").Select(MapToDto);
    }

    public async Task<FollowUpResponseDto> CreateFollowUp(CreateFollowUpDto dto)
    {
        var lead = await _leadRepo.GetByIdAsync(dto.LeadId);
        if (lead == null) throw new NotFoundException("Lead not found.");

        var currentUserId = _userManager.GetCurrentUserId();
        if (!currentUserId.HasValue) throw new UnauthorizedException("User not authenticated.");

        // Assign to current user if lead is assigned to them, or to lead's assigned user
        int assignedToId = lead.AssignedToUserId ?? currentUserId.Value;

        var followUp = new FollowUp
        {
            LeadId = dto.LeadId,
            Title = dto.Title,
            DueDate = dto.DueDate,
            Priority = string.IsNullOrWhiteSpace(dto.Priority) ? "Medium" : dto.Priority,
            Status = "Pending",
            AssignedToUserId = assignedToId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userManager.GetCurrentUserName() ?? _userManager.GetCurrentUserEmail() ?? "User",
            IsActive = true
        };

        var created = await _followUpRepo.AddAsync(followUp);
        await _auditLogService.LogAsync(
            "FollowUp",
            created.Id,
            "create",
            dto,
            currentUserId);
        var fetched = await _followUpRepo.GetByIdAsync(created.Id);
        return MapToDto(fetched ?? created);
    }

    public async Task<bool> UpdateStatus(int id, string status)
    {
        var followUp = await _followUpRepo.GetByIdAsync(id);
        if (followUp == null) throw new NotFoundException("FollowUp not found.");

        followUp.Status = status;
        followUp.UpdatedAt = DateTime.UtcNow;
        followUp.UpdatedBy = _userManager.GetCurrentUserEmail() ?? "User";

        await _followUpRepo.UpdateAsync(followUp);
        await _auditLogService.LogAsync(
            "FollowUp",
            followUp.Id,
            "update",
            new { followUp.Status },
            _userManager.GetCurrentUserId());
        return true;
    }

    public async Task<bool> DeleteFollowUp(int id)
    {
        var followUp = await _followUpRepo.GetByIdAsync(id);
        if (followUp == null)
        {
            return false;
        }

        var deleted = await _followUpRepo.DeleteAsync(id);
        if (deleted)
        {
            await _auditLogService.LogAsync(
                "FollowUp",
                followUp.Id,
                "delete",
                new { followUp.LeadId, followUp.Title, followUp.Status },
                _userManager.GetCurrentUserId());
        }

        return deleted;
    }
}
