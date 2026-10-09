using Backend.Application.Dtos.Lead;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using Backend.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Application.Dtos.Common;



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
}

public class LeadService: ILeadService
{
    private readonly ILeadRepository _leadRepository;
    private readonly IContactRepository _contactRepository;

    public LeadService(ILeadRepository leadRepository, IContactRepository contactRepository)
    {
        _leadRepository = leadRepository ?? throw new ArgumentNullException(nameof(leadRepository));
        _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(_contactRepository));
    }

    // Get single lead by ID
    public async Task<LeadResponseDto?> GetByIdAsync(int id)
    {
        var lead = await _leadRepository.GetByIdWithDetailsAsync(id);
        if (lead == null)
        {
            return null;
        }

        return lead.Adapt<LeadResponseDto>();
    }

    // Get all leads
    public async Task<IEnumerable<LeadResponseDto>> GetAllAsync()
    {
        var leads = await _leadRepository.GetAllAsync();
        return leads.Adapt<LeadResponseDto[]>();
    }

    // Get paged list of leads
    public async Task<(IEnumerable<LeadResponseDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _leadRepository.GetPagedAsync(pageNumber, pageSize);
        var dtos = items.Adapt<LeadResponseDto[]>();

        return (dtos, totalCount);
    }

    // Get leads filtered by stage
    public async Task<IEnumerable<LeadResponseDto>> GetByStageIdAsync(int stageId)
    {
        var leads = await _leadRepository.GetByStageIdAsync(stageId);
        return leads.Adapt<LeadResponseDto[]>();
    }

    // Create / Register a new lead
    public async Task<LeadResponseDto> RegisterAsync(CreateLeadDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var lead = dto.Adapt<Lead>();

        // Check that a contact exists
        var doesExists = await _contactRepository.ExistsAsync(lead.ContactId);
        if (!doesExists)
            throw new NotFoundException("Contact not found");

        lead.LeadStageId = 1; // Pending
        Console.WriteLine($"Leadster {lead.AssignedToUserId}");

        var createdLead = await _leadRepository.AddAsync(lead);
        return createdLead.Adapt<LeadResponseDto>();
    }
    public async Task<Lead?> RegisterWithContactAsync(CreateLeadWithContactDto dto)
    {
        var contact = await _contactRepository.AddAsync(dto.Adapt<Contact>());

        var lead = await _leadRepository.AddAsync(dto.Adapt<Lead>());
        lead.ContactId = contact.Id;

        await _leadRepository.UpdateAsync(lead);
        return lead;
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

        dto.Adapt(existingLead);



        await _leadRepository.UpdateAsync(existingLead);
        return true;
    }

    // Delete a lead
    public async Task<bool> DeleteAsync(int id)
    {
        return await _leadRepository.DeleteAsync(id);
    }

   
}
