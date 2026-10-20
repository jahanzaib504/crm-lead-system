using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Lead;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Api.Controllers;

[ApiController]
[Route("leads")]
public class LeadController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<bool>> CreateLeadWithContact([FromBody] CreateLeadWithContactDto dto)
    {
        await _leadService.RegisterWithContactAsync(dto);
        return Ok(true);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<LeadResponseDto>> CreateLead([FromBody] CreateLeadDto leadDto)
    {
        var res = await _leadService.RegisterAsync(leadDto);
        return Ok(res);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<LeadResponseDto>> GetLead(int id)
    {
        var res = await _leadService.GetByIdAsync(id);
        if (res == null) return NotFound();
        return Ok(res);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResponseDto<LeadResponseDto>>> GetLeadPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? stageId = null,
        [FromQuery] bool? openPool = null)
    {
        var res = await _leadService.GetPagedAsync(pageNumber, pageSize, search, stageId, openPool);
        return Ok(res);
    }

    [HttpGet("pool")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<IEnumerable<LeadResponseDto>>> GetOpenPoolLeads()
    {
        var res = await _leadService.GetOpenPoolLeads();
        return Ok(res);
    }

    [HttpPost("{id}/acquire")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<bool>> AcquireLead(int id)
    {
        var res = await _leadService.AcquireLead(id);
        return Ok(res);
    }

    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<bool>> AssignLead(int id, [FromBody] AssignLeadDto dto)
    {
        var res = await _leadService.AssignLead(id, dto);
        return Ok(res);
    }

    [HttpPatch("{id}/stage")]
    [Authorize]
    public async Task<ActionResult<bool>> UpdateLeadStage(int id, [FromBody] UpdateLeadStageDto dto)
    {
        var res = await _leadService.UpdateStageAsync(id, dto.StageId);
        return Ok(res);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<bool>> UpdateLead(int id, [FromBody] UpdateLeadDto dto)
    {
        var res = await _leadService.UpdateAsync(id, dto);
        return Ok(res);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> DeleteLead(int id)
    {
        var res = await _leadService.DeleteAsync(id);
        return Ok(res);
    }
}
