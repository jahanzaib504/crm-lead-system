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

    public LeadService(
        ILeadRepository leadRepository,
        IContactRepository contactRepository,
        IUserRepository userRepository,
        IUserManager userManager)
    {
        _leadRepository = leadRepository ?? throw new ArgumentNullException(nameof(leadRepository));
        _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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

        var list = query.OrderByDescending(l => l.CreatedAt).ToList();
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
        var openLeads = leads.Where(l => l.ManagerId == null && l.AssignedToUserId == null && IsUserAuthorizedForLead(l));
        return openLeads.OrderByDescending(l => l.CreatedAt).Select(MapToDto);
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

        await _leadRepository.UpdateAsync(existingLead);
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

        await _leadRepository.UpdateAsync(lead);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingLead = await _leadRepository.GetByIdAsync(id);
        if (existingLead == null) return false;

        var currentRole = _userManager.GetCurrentUserRole();
        if (currentRole != "Admin")
            throw new UnauthorizedException("Only administrators can delete leads.");

        return await _leadRepository.DeleteAsync(id);
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

        await _leadRepository.UpdateAsync(lead);
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

            await _leadRepository.UpdateAsync(lead);
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

            await _leadRepository.UpdateAsync(lead);
            return true;
        }

        throw new UnauthorizedException("You are not authorized to assign leads.");
    }
}