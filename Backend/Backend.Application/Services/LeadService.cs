using Backend.Application.Dtos.Lead;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using Backend.Application.Exceptions;
using Backend.Application.Dtos.Common;
using Microsoft.AspNetCore.Authorization;


namespace Backend.Application.Services;

public interface ILeadService
{
    Task<LeadResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<LeadResponseDto>> GetAllAsync();
    Task<(IEnumerable<LeadResponseDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<LeadResponseDto>> GetByStageIdAsync(int stageId);
    Task<LeadResponseDto> RegisterAsync(CreateLeadDto dto);
    Task<LeadResponseDto> RegisterWithContactAsync(CreateLeadWithContactDto dto);
    Task<bool> UpdateAsync(int id, UpdateLeadDto dto);
    Task<bool> DeleteAsync(int id);

    Task<bool> AcqireLead(int  id);
}

public class LeadService : ILeadService
{
    private readonly ILeadRepository _leadRepository;
    private readonly IContactRepository _contactRepository;
    private readonly IUserManager _userManager;

    public LeadService(ILeadRepository leadRepository, IContactRepository contactRepository, IUserManager userManager)
    {
        _leadRepository = leadRepository ?? throw new ArgumentNullException(nameof(leadRepository));
        _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    // Helper method to check access permission for a lead
    private bool IsUserAuthorizedForLead(Lead lead, int? currentUserId)
    {
        return lead.AssignedToUserId == currentUserId ||
               lead.ManagerId == currentUserId ||
               (lead.ManagerId == null && lead.AssignedToUserId == null);
    }

    // Get single lead by ID
    public async Task<LeadResponseDto?> GetByIdAsync(int id)
    {
        var lead = await _leadRepository.GetByIdAsync(id);
        if (lead == null)
        {
            return null;
        }

        var userId = _userManager.GetCurrentUserId();

        if (!IsUserAuthorizedForLead(lead, userId))
        {
            throw new UnauthorizedException("User is not allowed to access");
        }

        return lead.Adapt<LeadResponseDto>();
    }

    // Get all leads (filtered by user authorization)
    public async Task<IEnumerable<LeadResponseDto>> GetAllAsync()
    {
        var userId = _userManager.GetCurrentUserId();
        var leads = await _leadRepository.GetAllAsync();

        // Filter leads based on authorization criteria
        var authorizedLeads = leads.Where(lead => IsUserAuthorizedForLead(lead, userId));

        return authorizedLeads.Adapt<LeadResponseDto[]>();
    }

    // Get paged list of leads (filtered by user authorization)
    public async Task<(IEnumerable<LeadResponseDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var userId = _userManager.GetCurrentUserId();

        // Fetch all leads from the repository
        var leads = await _leadRepository.GetAllAsync();

        // Filter authorized leads first
        var authorizedLeads = leads.Where(lead => IsUserAuthorizedForLead(lead, userId)).ToList();

        int totalCount = authorizedLeads.Count;

        // Apply pagination in-memory on the authorized subset
        var pagedItems = authorizedLeads
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var dtos = pagedItems.Adapt<LeadResponseDto[]>();

        return (dtos, totalCount);
    }

    // Get leads filtered by stage (filtered by user authorization)
    public async Task<IEnumerable<LeadResponseDto>> GetByStageIdAsync(int stageId)
    {
        var userId = _userManager.GetCurrentUserId();
        var leads = await _leadRepository.GetByStageIdAsync(stageId);

        // Filter stage leads based on authorization criteria
        var authorizedLeads = leads.Where(lead => IsUserAuthorizedForLead(lead, userId));

        return authorizedLeads.Adapt<LeadResponseDto[]>();
    }

    // Create / Register a new lead
    public async Task<LeadResponseDto> RegisterAsync(CreateLeadDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var lead = dto.Adapt<Lead>();

        var doesExists = await _contactRepository.ExistsAsync(lead.ContactId);
        if (!doesExists)
            throw new NotFoundException("Contact not found");

        lead.LeadStageId = 1; // Pending
        Console.WriteLine($"Leadster {lead.AssignedToUserId}");

        var createdLead = await _leadRepository.AddAsync(lead);
        return createdLead.Adapt<LeadResponseDto>();
    }

    public async Task<LeadResponseDto> RegisterWithContactAsync(CreateLeadWithContactDto dto)
    {
        var doesEmailExists = await _contactRepository.ExistsByEmailAsync(dto.Email);

        if (doesEmailExists)
            throw new ConflictException("This email already exists");

        var contact = await _contactRepository.AddAsync(dto.Adapt<Contact>());

        var lead = dto.Adapt<Lead>();
        lead.ContactId = contact.Id;
        lead.LeadStageId = 1;
        await _leadRepository.AddAsync(lead);
        return lead.Adapt<LeadResponseDto>();
    }

    // Update an existing lead
    public async Task<bool> UpdateAsync(int id, UpdateLeadDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var existingLead = await _leadRepository.GetByIdAsync(id);
        if (existingLead == null)
        {
            return false;
        }

        var userId = _userManager.GetCurrentUserId();
        if (!IsUserAuthorizedForLead(existingLead, userId))
        {
            throw new UnauthorizedAccessException("User is not allowed to edit this lead");
        }

        dto.Adapt(existingLead);

        await _leadRepository.UpdateAsync(existingLead);
        return true;
    }

    // Delete a lead
    public async Task<bool> DeleteAsync(int id)
    {
        var existingLead = await _leadRepository.GetByIdAsync(id);
        if (existingLead == null)
        {
            return false;
        }

        var userId = _userManager.GetCurrentUserId();
        if (!IsUserAuthorizedForLead(existingLead, userId))
        {
            throw new UnauthorizedAccessException("User is not allowed to delete this lead");
        }

        return await _leadRepository.DeleteAsync(id);
    }
    [Authorize(Roles = "Sales Manager")]
    public async Task<bool> AcqireLead(int id)
    {
        // Allows sales manager to acquire lead
        var lead = await _leadRepository.GetByIdAsync(id);
        if(lead == null)
            throw new NotFoundException("Lead not found");

        if (lead.ManagerId != null)
            throw new UnauthorizedException("Lead is already acquired by a manager");

        lead.ManagerId = _userManager.GetCurrentUserId();
        await _leadRepository.UpdateAsync(lead);
        return true;
    }
}