
using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Note;
using Backend.Application.Exceptions;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Services;

public interface INoteService
{
    public Task<NoteResponseDto> GetNote(int noteId);
    public Task<NoteResponseDto> CreateNote(CreateNoteDto dto);
    public Task<PagedResponseDto<NoteResponseDto>> GetPagedNotes(NotePagedRequestDto dto);
}
public class NoteService: INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IUserManager _userManager;
    private readonly ILeadRepository _leadRepositry;
    public NoteService(INoteRepository noteRepository, IUserManager userManager, ILeadRepository leadRepository)
    {
        _noteRepository = noteRepository;
        _userManager= userManager;
        _leadRepositry = leadRepository;
    }
    public async Task<NoteResponseDto?> GetNote(int noteId)
    {
        var note = await _noteRepository.GetByIdWithLeadAsync(noteId);
        if (note == null)
            throw new NotFoundException("Note not found");

        int? userId = _userManager.GetCurrentUserId();
        if(note.Lead.AssignedToUserId != userId)
            throw new UnauthorizedException("User not allowed to access this");

        return note.Adapt<NoteResponseDto>();
    }
    public async Task<PagedResponseDto<NoteResponseDto>> GetPagedNotes(NotePagedRequestDto dto)
    {
        // Check if the lead belongs to user
        var lead = await _leadRepositry.GetByIdAsync(dto.LeadId);
        var currentUserId = _userManager.GetCurrentUserId();
        if (lead == null)
            throw new NotFoundException("Lead not found");

        if (currentUserId != lead.AssignedToUserId)
            throw new UnauthorizedException("User not allowed to access this lead");
        

        var notes = await _noteRepository.GetAllWithLeadsAsync();
        

        var authorizedNotes = notes.Where(n => n.LeadId == lead.Id && n.Lead.AssignedToUserId == currentUserId || n.Lead.ManagerId == currentUserId);

        var pagedNotes = authorizedNotes.Skip(dto.PageNumber - 1).Take(dto.PageSize);
        
        var totalCount = authorizedNotes.Count();

        return new PagedResponseDto<NoteResponseDto> { Items = pagedNotes.Adapt<NoteResponseDto[]>(),  TotalCount = totalCount};
    }
    public async Task<NoteResponseDto> CreateNote(CreateNoteDto dto)
    {
        // Create note with lead
        // check if the lead belongs to user
        var lead = await _leadRepositry.GetByIdAsync(dto.LeadId);
        if (lead == null)
            throw new NotFoundException("Lead not found");

        var userId = _userManager.GetCurrentUserId();
        if (userId != lead.AssignedToUserId)
            throw new UnauthorizedException("User not allowed to access this lead");

        var note = await _noteRepository.AddAsync(dto.Adapt<Note>());

        return note.Adapt<NoteResponseDto>();
    }
}
