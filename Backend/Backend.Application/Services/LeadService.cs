using Backend.Application.Dtos;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Backend.Application.Services;

public interface ILeadService
{
    Task<LeadDto?> GetByIdAsync(int id);
    Task<IEnumerable<LeadDto>> GetAllAsync();
    Task<(IEnumerable<LeadDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<LeadDto>> GetByStageIdAsync(int stageId);
    Task<LeadDto> RegisterAsync(CreateLeadDto dto);
    Task<bool> UpdateAsync(int id, UpdateLeadDto dto);
    Task<bool> DeleteAsync(int id);
}

public class LeadService: ILeadService
{
    private readonly ILeadRepository _leadRepository;

    public LeadService(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository ?? throw new ArgumentNullException(nameof(leadRepository));
    }

    // Get single lead by ID
    public async Task<LeadDto?> GetByIdAsync(int id)
    {
        var lead = await _leadRepository.GetByIdWithDetailsAsync(id);
        if (lead == null)
        {
            return null;
        }

        return lead.Adapt<LeadDto>();
    }

    // Get all leads
    public async Task<IEnumerable<LeadDto>> GetAllAsync()
    {
        var leads = await _leadRepository.GetAllAsync();
        return leads.Adapt<LeadDto[]>();
    }

    // Get paged list of leads
    public async Task<(IEnumerable<LeadDto> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var (items, totalCount) = await _leadRepository.GetPagedAsync(pageNumber, pageSize);
        var dtos = items.Adapt<LeadDto[]>();

        return (dtos, totalCount);
    }

    // Get leads filtered by stage
    public async Task<IEnumerable<LeadDto>> GetByStageIdAsync(int stageId)
    {
        var leads = await _leadRepository.GetByStageIdAsync(stageId);
        return leads.Adapt<LeadDto[]>();
    }

    // Create / Register a new lead
    public async Task<LeadDto> RegisterAsync(CreateLeadDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var lead = new Lead
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            LeadStageId = dto.LeadStageId,
            CreatedAt = DateTime.UtcNow
        };

        var createdLead = await _leadRepository.AddAsync(lead);
        return MapToDto(createdLead);
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

        existingLead.Name = dto.Name;
        existingLead.Email = dto.Email;
        existingLead.Phone = dto.Phone;
        existingLead.LeadStageId = dto.LeadStageId;
        existingLead.UpdatedAt = DateTime.UtcNow;

        await _leadRepository.UpdateAsync(existingLead);
        return true;
    }

    // Delete a lead
    public async Task<bool> DeleteAsync(int id)
    {
        return await _leadRepository.DeleteAsync(id);
    }

    // --- Helper Mapping Methods ---
}
