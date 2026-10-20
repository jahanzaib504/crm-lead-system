using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Note;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Api.Controllers;

[ApiController]
[Route("notes")]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;

    public NoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<NoteResponseDto>> GetNote(int id)
    {
        var res = await _noteService.GetNote(id);
        return Ok(res);
    }

    [HttpGet("lead/{leadId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<NoteResponseDto>>> GetNotesForLead(int leadId)
    {
        var res = await _noteService.GetNotesForLead(leadId);
        return Ok(res);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResponseDto<NoteResponseDto>>> GetPagedNotes([FromQuery] NotePagedRequestDto dto)
    {
        var res = await _noteService.GetPagedNotes(dto);
        return Ok(res);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<NoteResponseDto>> CreateNote([FromBody] CreateNoteDto dto)
    {
        var res = await _noteService.CreateNote(dto);
        return Ok(res);
    }
}
