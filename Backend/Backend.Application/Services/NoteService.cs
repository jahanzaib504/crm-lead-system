using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Note;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Application.Services;

public interface INoteService
{
    Task<NoteResponseDto> GetNote(int noteId);
    Task<NoteResponseDto> CreateNote(CreateNoteDto dto);
    Task<IEnumerable<NoteResponseDto>> GetNotesForLead(int leadId);
    Task<PagedResponseDto<NoteResponseDto>> GetPagedNotes(NotePagedRequestDto dto);
}

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IUserManager _userManager;
    private readonly ILeadRepository _leadRepository;

    public NoteService(INoteRepository noteRepository, IUserManager userManager, ILeadRepository leadRepository)
    {
        _noteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _leadRepository = leadRepository ?? throw new ArgumentNullException(nameof(leadRepository));
    }

    private bool IsAuthorizedForLead(Lead lead)
    {
        var role = _userManager.GetCurrentUserRole();
        var userId = _userManager.GetCurrentUserId();

        if (role == "Admin") return true;
        if (role == "Sales Manager") return lead.ManagerId == userId || (lead.ManagerId == null && lead.AssignedToUserId == null);
        return lead.AssignedToUserId == userId;
    }

    public async Task<NoteResponseDto> GetNote(int noteId)
    {
        var note = await _noteRepository.GetByIdWithLeadAsync(noteId);
        if (note == null)
            throw new NotFoundException("Note not found.");

        if (!IsAuthorizedForLead(note.Lead))
            throw new UnauthorizedException("User not authorized to access this note.");

        return note.Adapt<NoteResponseDto>();
    }

    public async Task<IEnumerable<NoteResponseDto>> GetNotesForLead(int leadId)
    {
        var lead = await _leadRepository.GetByIdAsync(leadId);
        if (lead == null)
            throw new NotFoundException("Lead not found.");

        if (!IsAuthorizedForLead(lead))
            throw new UnauthorizedException("User not authorized to access notes for this lead.");

        var allNotes = await _noteRepository.GetAllAsync();
        var leadNotes = allNotes
            .Where(n => n.LeadId == leadId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => n.Adapt<NoteResponseDto>());

        return leadNotes;
    }

    public async Task<PagedResponseDto<NoteResponseDto>> GetPagedNotes(NotePagedRequestDto dto)
    {
        var lead = await _leadRepository.GetByIdAsync(dto.LeadId);
        if (lead == null)
            throw new NotFoundException("Lead not found.");

        if (!IsAuthorizedForLead(lead))
            throw new UnauthorizedException("User not authorized to access notes for this lead.");

        var allNotes = await _noteRepository.GetAllAsync();
        var filtered = allNotes.Where(n => n.LeadId == dto.LeadId).OrderByDescending(n => n.CreatedAt).ToList();

        var totalCount = filtered.Count;
        var paged = filtered.Skip((dto.PageNumber - 1) * dto.PageSize).Take(dto.PageSize).Select(n => n.Adapt<NoteResponseDto>()).ToArray();

        return new PagedResponseDto<NoteResponseDto> { Items = paged, TotalCount = totalCount };
    }

    public async Task<NoteResponseDto> CreateNote(CreateNoteDto dto)
    {
        var lead = await _leadRepository.GetByIdAsync(dto.LeadId);
        if (lead == null)
            throw new NotFoundException("Lead not found.");

        if (!IsAuthorizedForLead(lead))
            throw new UnauthorizedException("User not authorized to create notes for this lead.");

        var note = new Note
        {
            LeadId = dto.LeadId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userManager.GetCurrentUserName() ?? _userManager.GetCurrentUserEmail() ?? "User",
            IsActive = true
        };

        var created = await _noteRepository.AddAsync(note);
        return created.Adapt<NoteResponseDto>();
    }
}
