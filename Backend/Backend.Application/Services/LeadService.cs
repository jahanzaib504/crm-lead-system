using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Lead;
using Backend.Application.Dtos.LeadStage;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Services;

public interface ILeadService
{
    Task<LeadResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<LeadResponseDto>> GetAllAsync();
    Task<PagedResponseDto<LeadResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search = null, int? stageId = null, bool? openPool = null);
    Task<IEnumerable<LeadResponseDto>> GetOpenPoolLeads();
    Task<IEnumerable<LeadResponseDto>> GetByStageIdAsync(int stageId);
    Task<LeadResponseDto> RegisterAsync(CreateLeadDto dto);
    Task<LeadResponseDto> RegisterWithContactAsync(CreateLeadWithContactDto dto);
    Task<bool> UpdateAsync(int id, UpdateLeadDto dto);
    Task<bool> UpdateStageAsync(int id, int stageId);
    Task<bool> DeleteAsync(int id);
    Task<bool> AcquireLead(int id);
    Task<bool> AssignLead(int id, int salesRepId);
    Task<bool> AssignLead(int id, AssignLeadDto dto);
}

public class LeadService : ILeadService
{
    private readonly ILeadRepository _leadRepository;
    private readonly IContactRepository _contactRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserManager _userManager;
    private readonly AuditLogService _auditLogService;

    public LeadService(
        ILeadRepository leadRepository,
        IContactRepository contactRepository,
        IUserRepository userRepository,
        IUserManager userManager,
        AuditLogService auditLogService)
    {
        _leadRepository = leadRepository ?? throw new ArgumentNullException(nameof(leadRepository));
        _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
    }

    private static LeadResponseDto MapToDto(Lead lead)
    {
        return new LeadResponseDto
        {
            Id = lead.Id,
            Title = lead.Title,
            Query = lead.Query,
            ContactId = lead.ContactId,
            CompanyId = lead.CompanyId,
            LeadStageId = lead.LeadStageId,
            AssignedToUserId = lead.AssignedToUserId,
            AssignedToUserName = lead.AssignedToUser?.FullName,
            ManagerId = lead.ManagerId,
            ManagerName = lead.Manager?.FullName,
            Source = lead.Source,
            EstimatedValue = lead.EstimatedValue,
            CurrentScore = lead.CurrentScore,
            ContactName = lead.Contact != null ? $"{lead.Contact.FirstName} {lead.Contact.LastName}".Trim() : null,
            ContactEmail = lead.Contact?.Email,
            ContactPhone = lead.Contact?.Phone,
            ContactJobTitle = lead.Contact?.JobTitle,
            CompanyName = lead.Company?.Name,
            CreatedAt = lead.CreatedAt,
            UpdatedAt = lead.UpdatedAt,
            LeadStage = lead.LeadStage != null ? new LeadStageDto
            {
                Id = lead.LeadStage.Id,
                Name = lead.LeadStage.Name,
                DisplayOrder = lead.LeadStage.DisplayOrder,
                ConversionWeight = lead.LeadStage.ConversionWeight
            } : null
        };
    }

    private static bool IsClosedWonLead(Lead lead)
    {
        if (lead == null) return false;

        if (lead.LeadStage != null)
        {
            return string.Equals(lead.LeadStage.Name, "Closed Won", StringComparison.OrdinalIgnoreCase);
        }

        return lead.LeadStageId == 5;
    }

    private static decimal CalculateLeadScore(Lead lead, IEnumerable<Lead> allLeads)
    {
        if (lead == null) return 0m;

        var historicalLeads = allLeads ?? Enumerable.Empty<Lead>();
        var sameContactHistory = historicalLeads
            .Where(l => l.Id != lead.Id && l.ContactId == lead.ContactId)
            .ToList();

        var sameCompanyHistory = historicalLeads
            .Where(l => l.Id != lead.Id && l.CompanyId.HasValue && lead.CompanyId.HasValue && l.CompanyId == lead.CompanyId)
            .ToList();

        var contactWinRate = sameContactHistory.Count > 0
            ? sameContactHistory.Count(l => IsClosedWonLead(l)) / (decimal)sameContactHistory.Count
            : 0m;

        var companyWinRate = sameCompanyHistory.Count > 0
            ? sameCompanyHistory.Count(l => IsClosedWonLead(l)) / (decimal)sameCompanyHistory.Count
            : 0m;

        var recentWins = sameContactHistory
            .Concat(sameCompanyHistory)
            .DistinctBy(l => l.Id)
            .Count(l => IsClosedWonLead(l) && l.CreatedAt >= DateTime.UtcNow.AddDays(-90));

        decimal score = 0m;

        score += Math.Min(contactWinRate * 35m, 35m);
        score += Math.Min(companyWinRate * 25m, 25m);
        score += Math.Min(sameContactHistory.Count(l => IsClosedWonLead(l)) * 10m, 20m);
        score += Math.Min(sameCompanyHistory.Count(l => IsClosedWonLead(l)) * 8m, 15m);
        score += Math.Min(recentWins * 4m, 15m);

        if (lead.EstimatedValue.HasValue)
        {
            score += Math.Min(lead.EstimatedValue.Value / 10000m, 12m);
        }

        if (lead.LeadStage != null)
        {
            score += Math.Min((lead.LeadStage.ConversionWeight / 10m) * 2m, 12m);
        }
        else if (lead.LeadStageId > 0)
        {
            score += Math.Min(lead.LeadStageId * 1.5m, 12m);
        }

        if (!string.IsNullOrWhiteSpace(lead.Contact?.Email))
        {
            score += 5m;
        }

        if (!string.IsNullOrWhiteSpace(lead.Company?.Name))
        {
            score += 5m;
        }

        if (!string.IsNullOrWhiteSpace(lead.Query))
        {
            score += 3m;
        }

        return Math.Clamp(score, 0m, 100m);
    }

    private bool IsUserAuthorizedForLead(Lead lead)
    {
        int? currentUserId = _userManager.GetCurrentUserId();
        string? currentUserRole = _userManager.GetCurrentUserRole();

        if (string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase)) return true;

        if (string.Equals(currentUserRole, "Sales Manager", StringComparison.OrdinalIgnoreCase))
        {
            return (currentUserId.HasValue && lead.ManagerId == currentUserId.Value) || 
                   (lead.ManagerId == null && lead.AssignedToUserId == null);
        }

        return currentUserId.HasValue && lead.AssignedToUserId == currentUserId.Value;
    }

    public async Task<LeadResponseDto?> GetByIdAsync(int id)
    {
        var lead = await _leadRepository.GetByIdAsync(id);
        if (lead == null) return null;

        if (!IsUserAuthorizedForLead(lead))
        {
            throw new UnauthorizedException("User is not authorized to access this lead.");
        }

        return MapToDto(lead);
    }

    public async Task<IEnumerable<LeadResponseDto>> GetAllAsync()
    {
        var leads = await _leadRepository.GetAllAsync();
        var authorizedLeads = leads.Where(IsUserAuthorizedForLead);
        return authorizedLeads.Select(MapToDto);
    }

    public async Task<PagedResponseDto<LeadResponseDto>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        int? stageId = null,
        bool? openPool = null)
    {
        var leads = await _leadRepository.GetAllAsync();
        var query = leads.Where(IsUserAuthorizedForLead);

        if (openPool == true)
        {
            query = query.Where(l => l.ManagerId == null && l.AssignedToUserId == null);
        }
        else if (openPool == false)
        {
            query = query.Where(l => l.ManagerId != null || l.AssignedToUserId != null);
        }

        if (stageId.HasValue && stageId.Value > 0)
        {
            query = query.Where(l => l.LeadStageId == stageId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(l =>
                (l.Title != null && l.Title.ToLowerInvariant().Contains(s)) ||
                (l.Query != null && l.Query.ToLowerInvariant().Contains(s)) ||
                (l.Contact != null && (l.Contact.FirstName.ToLowerInvariant().Contains(s) || l.Contact.LastName.ToLowerInvariant().Contains(s) || l.Contact.Email.ToLowerInvariant().Contains(s))) ||
                (l.Company != null && l.Company.Name.ToLowerInvariant().Contains(s)));
        }

        var fullLeadHistory = await _leadRepository.GetAllAsync();

        var list = query
            .Select(l =>
            {
                l.CurrentScore = CalculateLeadScore(l, fullLeadHistory);
                return l;
            })
            .OrderByDescending(l => l.CurrentScore ?? 0m)
            .ThenByDescending(l => l.EstimatedValue ?? 0m)
            .ThenByDescending(l => l.CreatedAt)
            .ToList();

        var totalCount = list.Count;
        var pagedItems = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(MapToDto).ToArray();

        return new PagedResponseDto<LeadResponseDto>
        {
            Items = pagedItems,
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<LeadResponseDto>> GetOpenPoolLeads()
    {
        var leads = await _leadRepository.GetAllAsync();
        var openLeads = leads
            .Where(l => l.ManagerId == null && l.AssignedToUserId == null && IsUserAuthorizedForLead(l))
            .Select(l =>
            {
                l.CurrentScore = CalculateLeadScore(l, leads);
                return l;
            })
            .OrderByDescending(l => l.CurrentScore ?? 0m)
            .ThenByDescending(l => l.EstimatedValue ?? 0m)
            .ThenByDescending(l => l.CreatedAt)
            .Select(MapToDto);

        return openLeads;
    }

    public async Task<IEnumerable<LeadResponseDto>> GetByStageIdAsync(int stageId)
    {
        var leads = await _leadRepository.GetByStageIdAsync(stageId);
        return leads.Where(IsUserAuthorizedForLead).Select(MapToDto);
    }

    public async Task<LeadResponseDto> RegisterAsync(CreateLeadDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var doesExist = await _contactRepository.ExistsAsync(dto.ContactId);
        if (!doesExist)
            throw new NotFoundException("Contact not found.");

        var lead = new Lead
        {
            Title = dto.Title,
            Query = dto.Query,
            ContactId = dto.ContactId,
            CompanyId = dto.CompanyId,
            LeadStageId = 1, // New
            Source = dto.Source,
            EstimatedValue = dto.EstimatedValue,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userManager.GetCurrentUserEmail() ?? "System"
        };

        var createdLead = await _leadRepository.AddAsync(lead);
        await _auditLogService.LogAsync(
            "Lead",
            createdLead.Id,
            "create",
            dto,
            _userManager.GetCurrentUserId());
        var fetched = await _leadRepository.GetByIdAsync(createdLead.Id);
        return MapToDto(fetched ?? createdLead);
    }

    public async Task<LeadResponseDto> RegisterWithContactAsync(CreateLeadWithContactDto dto)
    {
        var doesEmailExist = await _contactRepository.ExistsByEmailAsync(dto.Email);
        Contact contact;

        if (doesEmailExist)
        {
            // If contact exists, reuse existing contact or update
            var contacts = await _contactRepository.GetAllAsync();
            contact = contacts.First(c => c.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            var newContact = new Contact
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                JobTitle = dto.JobTitle,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "PublicForm"
            };
            contact = await _contactRepository.AddAsync(newContact);
        }

        var lead = new Lead
        {
            Title = dto.Title,
            Query = dto.Query,
            ContactId = contact.Id,
            LeadStageId = 1, // New
            Source = dto.Source,
            EstimatedValue = dto.EstimatedValue,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "PublicForm"
        };

        var created = await _leadRepository.AddAsync(lead);
        await _auditLogService.LogAsync(
            "Lead",
            created.Id,
            "create",
            dto,
            _userManager.GetCurrentUserId());
        var fetched = await _leadRepository.GetByIdAsync(created.Id);
        return MapToDto(fetched ?? created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateLeadDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var existingLead = await _leadRepository.GetByIdAsync(id);
        if (existingLead == null) return false;

        if (!IsUserAuthorizedForLead(existingLead))
            throw new UnauthorizedException("User is not authorized to edit this lead.");

        existingLead.Title = dto.Title ?? existingLead.Title;
        existingLead.Query = dto.Query ?? existingLead.Query;
        existingLead.Source = dto.Source ?? existingLead.Source;
        existingLead.EstimatedValue = dto.EstimatedValue ?? existingLead.EstimatedValue;
        if (dto.LeadStageId.HasValue) existingLead.LeadStageId = dto.LeadStageId.Value;

        existingLead.UpdatedAt = DateTime.UtcNow;
        existingLead.UpdatedBy = _userManager.GetCurrentUserEmail() ?? "User";

        var updatedLead = await _leadRepository.UpdateAsync(existingLead);
        await _auditLogService.LogAsync(
            "Lead",
            updatedLead.Id,
            "update",
            dto,
            _userManager.GetCurrentUserId());
        return true;
    }

    public async Task<bool> UpdateStageAsync(int id, int stageId)
    {
        var lead = await _leadRepository.GetByIdAsync(id);
        if (lead == null) throw new NotFoundException("Lead not found.");

        if (!IsUserAuthorizedForLead(lead))
            throw new UnauthorizedException("User not authorized to update this lead.");

        lead.LeadStageId = stageId;
        lead.UpdatedAt = DateTime.UtcNow;
        lead.UpdatedBy = _userManager.GetCurrentUserEmail() ?? "User";

        var updatedLead = await _leadRepository.UpdateAsync(lead);
        await _auditLogService.LogAsync(
            "Lead",
            updatedLead.Id,
            "update",
            new { StageId = stageId },
            _userManager.GetCurrentUserId());
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingLead = await _leadRepository.GetByIdAsync(id);
        if (existingLead == null) return false;

        var currentRole = _userManager.GetCurrentUserRole();
        if (currentRole != "Admin")
            throw new UnauthorizedException("Only administrators can delete leads.");

        var deleted = await _leadRepository.DeleteAsync(id);
        if (deleted)
        {
            await _auditLogService.LogAsync(
                "Lead",
                existingLead.Id,
                "delete",
                new { existingLead.Title, existingLead.ContactId, existingLead.CompanyId, existingLead.LeadStageId },
                _userManager.GetCurrentUserId());
        }

        return deleted;
    }

    public async Task<bool> AcquireLead(int id)
    {
        var currentRole = _userManager.GetCurrentUserRole();
        var currentUserId = _userManager.GetCurrentUserId();

        if (string.IsNullOrEmpty(currentRole) || !currentUserId.HasValue)
            throw new UnauthorizedException("User is not authenticated.");

        if (!string.Equals(currentRole, "Sales Manager", StringComparison.OrdinalIgnoreCase) && 
            !string.Equals(currentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedException("Only Sales Managers and Admins can acquire leads from the open pool.");

        var lead = await _leadRepository.GetByIdAsync(id);
        if (lead == null)
            throw new NotFoundException("Lead not found.");

        if (lead.ManagerId != null)
            throw new ConflictException("Lead has already been acquired.");

        lead.ManagerId = currentUserId.Value;
        lead.UpdatedAt = DateTime.UtcNow;
        lead.UpdatedBy = _userManager.GetCurrentUserEmail() ?? (string.Equals(currentRole, "Admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Manager");

        var updatedLead = await _leadRepository.UpdateAsync(lead);
        await _auditLogService.LogAsync(
            "Lead",
            updatedLead.Id,
            "update",
            new { lead.ManagerId },
            currentUserId);
        return true;
    }

    public async Task<bool> AssignLead(int id, int salesRepId)
    {
        return await AssignLead(id, new AssignLeadDto { SalesRepId = salesRepId });
    }

    public async Task<bool> AssignLead(int id, AssignLeadDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var currentRole = _userManager.GetCurrentUserRole();
        var currentUserId = _userManager.GetCurrentUserId();

        if (string.IsNullOrEmpty(currentRole) || !currentUserId.HasValue)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        var lead = await _leadRepository.GetByIdAsync(id);
        if (lead == null)
        {
            throw new NotFoundException("Lead not found.");
        }

        int targetUserId = dto.ManagerId ?? dto.UserId ?? dto.SalesRepId ?? 0;
        if (targetUserId <= 0)
        {
            throw new BadRequestException("A valid user ID must be specified for assignment.");
        }

        var targetUser = await _userRepository.GetByIdAsync(targetUserId);
        if (targetUser == null || !targetUser.IsActive)
        {
            throw new NotFoundException("Selected user does not exist or is not active.");
        }

        if (string.Equals(currentRole, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            // The admin can assign it to a manager
            if (targetUser.RoleId != 2)
            {
                throw new BadRequestException("Administrators can only assign leads to a Sales Manager. Selected user is not a Sales Manager.");
            }

            lead.ManagerId = targetUser.Id;

            // If lead previously had an assigned sales rep that does not report to the new manager, clear assigned rep
            if (lead.AssignedToUserId.HasValue)
            {
                var assignedRep = await _userRepository.GetByIdAsync(lead.AssignedToUserId.Value);
                if (assignedRep == null || assignedRep.ManagerId != targetUser.Id)
                {
                    lead.AssignedToUserId = null;
                }
            }

            lead.UpdatedAt = DateTime.UtcNow;
            lead.UpdatedBy = _userManager.GetCurrentUserEmail() ?? "Admin";

            var updatedLead = await _leadRepository.UpdateAsync(lead);
            await _auditLogService.LogAsync(
                "Lead",
                updatedLead.Id,
                "update",
                new { lead.ManagerId, lead.AssignedToUserId },
                currentUserId);
            return true;
        }

        if (string.Equals(currentRole, "Sales Manager", StringComparison.OrdinalIgnoreCase))
        {
            // Only then a manager can assign it to a sales rep
            if (lead.ManagerId == null)
            {
                throw new UnauthorizedException("This lead has not been acquired or assigned to you yet. You must acquire it before assigning to a representative.");
            }

            if (lead.ManagerId != currentUserId.Value)
            {
                throw new UnauthorizedException("You can only assign leads that are assigned to your team.");
            }

            if (targetUser.RoleId != 3)
            {
                throw new BadRequestException("Sales Managers can only assign leads to a Sales Representative.");
            }

            if (targetUser.ManagerId != currentUserId.Value)
            {
                throw new UnauthorizedException("You can only assign leads to sales representatives on your team.");
            }

            lead.AssignedToUserId = targetUser.Id;
            lead.UpdatedAt = DateTime.UtcNow;
            lead.UpdatedBy = _userManager.GetCurrentUserEmail() ?? "Manager";

            var updatedLead = await _leadRepository.UpdateAsync(lead);
            await _auditLogService.LogAsync(
                "Lead",
                updatedLead.Id,
                "update",
                new { lead.ManagerId, lead.AssignedToUserId },
                currentUserId);
            return true;
        }

        throw new UnauthorizedException("You are not authorized to assign leads.");
    }
}