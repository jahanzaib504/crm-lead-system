
using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Note;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("notes")]
public class NoteController: ControllerBase
{
    private readonly INoteService _noteService;
    public NoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<NoteResponseDto> GetNote(int id)
    {
        return await _noteService.GetNote(id);
    }
    [HttpGet]
    [Authorize]
    public async Task<PagedResponseDto<NoteResponseDto>> GetPagedNotes([FromQuery] NotePagedRequestDto dto)
    {
        return await _noteService.GetPagedNotes(dto);
    }
    //[HttpGet]
    //[Authorize]
    //public async Task<NoteResponseDto> GetPagedNote([FromQuery])
    //{
    //    return await _noteService.GetNote(id);
    //}

    [HttpPost]
    [Authorize]
    public async Task<NoteResponseDto> CreateNote(CreateNoteDto dto)
    {
        return await _noteService.CreateNote(dto);
    }
}
