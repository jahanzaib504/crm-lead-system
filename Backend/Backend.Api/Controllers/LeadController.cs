using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Api;
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
    public async Task<ActionResult<ApiResponse<bool>>> CreateLeadWithContact([FromBody] CreateLeadWithContactDto dto)
    {
        await _leadService.RegisterWithContactAsync(dto);
        var response = new ApiResponse<bool>(StatusCodes.Status201Created, "Lead created successfully", true);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LeadResponseDto>>> CreateLead([FromBody] CreateLeadDto leadDto)
    {
        var res = await _leadService.RegisterAsync(leadDto);
        var response = new ApiResponse<LeadResponseDto>(StatusCodes.Status201Created, "Lead created successfully", res);
        return CreatedAtAction(nameof(GetLead), new { id = res.Id }, response);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<LeadResponseDto>>> GetLead(int id)
    {
        var res = await _leadService.GetByIdAsync(id);
        if (res == null)
        {
            return NotFound(new ApiError { Status = StatusCodes.Status404NotFound, Message = "Lead not found." });
        }

        return Ok(new ApiResponse<LeadResponseDto>(StatusCodes.Status200OK, "Lead retrieved successfully", res));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<LeadResponseDto>>>> GetLeadPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? stageId = null,
        [FromQuery] bool? openPool = null)
    {
        var res = await _leadService.GetPagedAsync(pageNumber, pageSize, search, stageId, openPool);
        return Ok(new ApiResponse<PagedResponseDto<LeadResponseDto>>(StatusCodes.Status200OK, "Leads retrieved successfully", res));
    }

    [HttpGet("pool")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LeadResponseDto>>>> GetOpenPoolLeads()
    {
        var res = await _leadService.GetOpenPoolLeads();
        return Ok(new ApiResponse<IEnumerable<LeadResponseDto>>(StatusCodes.Status200OK, "Open pool leads retrieved successfully", res));
    }

    [HttpPost("{id}/acquire")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<ApiResponse<bool>>> AcquireLead(int id)
    {
        var res = await _leadService.AcquireLead(id);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Lead acquired successfully", res));
    }

    [HttpPost("{id}/withdraw")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<ApiResponse<bool>>> WithdrawLead(int id)
    {
        var res = await _leadService.WithdrawLead(id);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Lead withdrawn successfully", res));
    }

    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin,Sales Manager")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignLead(int id, [FromBody] AssignLeadDto dto)
    {
        var res = await _leadService.AssignLead(id, dto);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Lead assigned successfully", res));
    }

    [HttpPatch("{id}/stage")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateLeadStage(int id, [FromBody] UpdateLeadStageDto dto)
    {
        var res = await _leadService.UpdateStageAsync(id, dto.StageId);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Lead stage updated successfully", res));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateLead(int id, [FromBody] UpdateLeadDto dto)
    {
        var res = await _leadService.UpdateAsync(id, dto);
        if (!res)
        {
            return NotFound(new ApiError { Status = StatusCodes.Status404NotFound, Message = "Lead not found." });
        }

        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Lead updated successfully", res));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteLead(int id)
    {
        var res = await _leadService.DeleteAsync(id);
        if (!res)
        {
            return NotFound(new ApiError { Status = StatusCodes.Status404NotFound, Message = "Lead not found." });
        }

        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Lead deleted successfully", res));
    }
}
