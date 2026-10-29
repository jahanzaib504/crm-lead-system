using Backend.Application.Dtos.Analytics;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Services;

public interface IAnalyticsService
{
    Task<AnalyticsResponseDto> GetDashboardAnalytics();
}

public class AnalyticsService : IAnalyticsService
{
    private readonly ILeadRepository _leadRepo;
    private readonly IUserRepository _userRepo;
    private readonly IFollowUpRepository _followUpRepo;
    private readonly IActivityRepository _activityRepo;
    private readonly IUserManager _userManager;

    public AnalyticsService(
        ILeadRepository leadRepo,
        IUserRepository userRepo,
        IFollowUpRepository followUpRepo,
        IActivityRepository activityRepo,
        IUserManager userManager)
    {
        _leadRepo = leadRepo ?? throw new ArgumentNullException(nameof(leadRepo));
        _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        _followUpRepo = followUpRepo ?? throw new ArgumentNullException(nameof(followUpRepo));
        _activityRepo = activityRepo ?? throw new ArgumentNullException(nameof(activityRepo));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<AnalyticsResponseDto> GetDashboardAnalytics()
    {
        var role = _userManager.GetCurrentUserRole() ?? "Sales Rep";
        var userId = _userManager.GetCurrentUserId();

        var allLeads = (await _leadRepo.GetAllAsync()).ToList();
        var allUsers = (await _userRepo.GetAllAsync()).ToList();

        var response = new AnalyticsResponseDto
        {
            Role = role
        };

        if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            response.TotalLeads = allLeads.Count;
            response.OpenPoolLeads = allLeads.Count(l => l.ManagerId == null && l.AssignedToUserId == null);
            response.ConvertedLeads = allLeads.Count(l => l.LeadStageId == 5); // 5 = Closed Won
            response.TotalPipelineValue = allLeads.Sum(l => l.EstimatedValue ?? 0);
            response.ConversionRate = response.TotalLeads > 0 
                ? Math.Round((double)response.ConvertedLeads / response.TotalLeads * 100, 1) 
                : 0;

            response.TotalUsers = allUsers.Count;
            response.PendingApprovals = allUsers.Count(u => !u.RoleId.HasValue); // Users without a role are considered pending approval

            response.StageBreakdown = allLeads
                .GroupBy(l => l.LeadStage?.Name ?? "Stage " + l.LeadStageId)
                .Select(g => new StageCountDto { StageName = g.Key, Count = g.Count() })
                .ToList();

            response.SourceBreakdown = allLeads
                .GroupBy(l => string.IsNullOrWhiteSpace(l.Source) ? "Unknown" : l.Source)
                .Select(g => new SourceCountDto { Source = g.Key, Count = g.Count() })
                .ToList();
        }
        else if (string.Equals(role, "Sales Manager", StringComparison.OrdinalIgnoreCase))
        {
            var teamLeads = userId.HasValue 
                ? allLeads.Where(l => l.ManagerId == userId.Value).ToList() 
                : new List<Lead>();
            var teamReps = userId.HasValue 
                ? allUsers.Where(u => u.ManagerId == userId.Value && u.RoleId == 3 && u.IsActive).ToList() 
                : new List<User>();

            response.TeamLeadsCount = teamLeads.Count;
            response.OpenPoolLeads = allLeads.Count(l => l.ManagerId == null && l.AssignedToUserId == null);
            response.ActiveRepsCount = teamReps.Count;
            response.TeamPipelineValue = teamLeads.Sum(l => l.EstimatedValue ?? 0);
            response.ConvertedLeads = teamLeads.Count(l => l.LeadStageId == 5);
            response.ConversionRate = response.TeamLeadsCount > 0 
                ? Math.Round((double)response.ConvertedLeads / response.TeamLeadsCount * 100, 1) 
                : 0;

            response.RepPerformance = teamReps.Select(rep => new RepPerformanceDto
            {
                RepId = rep.Id,
                RepName = rep.FullName ?? rep.Email,
                LeadCount = teamLeads.Count(l => l.AssignedToUserId == rep.Id),
                TotalValue = teamLeads.Where(l => l.AssignedToUserId == rep.Id).Sum(l => l.EstimatedValue ?? 0)
            }).ToList();

            response.StageBreakdown = teamLeads
                .GroupBy(l => l.LeadStage?.Name ?? "Stage " + l.LeadStageId)
                .Select(g => new StageCountDto { StageName = g.Key, Count = g.Count() })
                .ToList();

            response.SourceBreakdown = teamLeads
                .GroupBy(l => string.IsNullOrWhiteSpace(l.Source) ? "Unknown" : l.Source)
                .Select(g => new SourceCountDto { Source = g.Key, Count = g.Count() })
                .ToList();
        }
        else // Sales Rep
        {
            var myLeads = userId.HasValue 
                ? allLeads.Where(l => l.AssignedToUserId == userId.Value).ToList() 
                : new List<Lead>();
            var myFollowUps = userId.HasValue 
                ? (await _followUpRepo.GetByUserIdAsync(userId.Value)).ToList() 
                : new List<FollowUp>();
            var myActivities = userId.HasValue 
                ? (await _activityRepo.GetByUserIdAsync(userId.Value)).ToList() 
                : new List<Activity>();

            response.MyLeadsCount = myLeads.Count;
            response.MyPipelineValue = myLeads.Sum(l => l.EstimatedValue ?? 0);
            response.PendingFollowUpsCount = myFollowUps.Count(f => f.Status == "Pending");
            response.CompletedActivitiesCount = myActivities.Count;
            response.ConvertedLeads = myLeads.Count(l => l.LeadStageId == 5);
            response.ConversionRate = response.MyLeadsCount > 0 
                ? Math.Round((double)response.ConvertedLeads / response.MyLeadsCount * 100, 1) 
                : 0;

            response.StageBreakdown = myLeads
                .GroupBy(l => l.LeadStage?.Name ?? "Stage " + l.LeadStageId)
                .Select(g => new StageCountDto { StageName = g.Key, Count = g.Count() })
                .ToList();

            response.SourceBreakdown = myLeads
                .GroupBy(l => string.IsNullOrWhiteSpace(l.Source) ? "Unknown" : l.Source)
                .Select(g => new SourceCountDto { Source = g.Key, Count = g.Count() })
                .ToList();
        }

        return response;
    }
}
