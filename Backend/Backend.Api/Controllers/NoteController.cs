using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Api;
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
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> GetNote(int id)
    {
        var res = await _noteService.GetNote(id);
        return Ok(new ApiResponse<NoteResponseDto>(StatusCodes.Status200OK, "Note retrieved successfully", res));
    }

    [HttpGet("lead/{leadId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<NoteResponseDto>>>> GetNotesForLead(int leadId)
    {
        var res = await _noteService.GetNotesForLead(leadId);
        return Ok(new ApiResponse<IEnumerable<NoteResponseDto>>(StatusCodes.Status200OK, "Notes retrieved successfully", res));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<NoteResponseDto>>>> GetPagedNotes([FromQuery] NotePagedRequestDto dto)
    {
        var res = await _noteService.GetPagedNotes(dto);
        return Ok(new ApiResponse<PagedResponseDto<NoteResponseDto>>(StatusCodes.Status200OK, "Notes retrieved successfully", res));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<NoteResponseDto>>> CreateNote([FromBody] CreateNoteDto dto)
    {
        var res = await _noteService.CreateNote(dto);
        var response = new ApiResponse<NoteResponseDto>(StatusCodes.Status201Created, "Note created successfully", res);
        return CreatedAtAction(nameof(GetNote), new { id = res.Id }, response);
    }
}
