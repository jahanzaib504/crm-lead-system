using Backend.Application.Dtos.FollowUp;
using Backend.Application.Dtos.Api;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Api.Controllers;

[ApiController]
[Route("followups")]
[Authorize]
public class FollowUpController : ControllerBase
{
    private readonly IFollowUpService _followUpService;

    public FollowUpController(IFollowUpService followUpService)
    {
        _followUpService = followUpService;
    }

    [HttpGet("lead/{leadId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FollowUpResponseDto>>>> GetByLead(int leadId)
    {
        var res = await _followUpService.GetByLeadId(leadId);
        return Ok(new ApiResponse<IEnumerable<FollowUpResponseDto>>(StatusCodes.Status200OK, "Follow-ups retrieved successfully", res));
    }

    [HttpGet("my-pending")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FollowUpResponseDto>>>> GetMyPending()
    {
        var res = await _followUpService.GetMyPendingFollowUps();
        return Ok(new ApiResponse<IEnumerable<FollowUpResponseDto>>(StatusCodes.Status200OK, "Pending follow-ups retrieved successfully", res));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FollowUpResponseDto>>> Create([FromBody] CreateFollowUpDto dto)
    {
        var res = await _followUpService.CreateFollowUp(dto);
        var response = new ApiResponse<FollowUpResponseDto>(StatusCodes.Status201Created, "Follow-up created successfully", res);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] UpdateFollowUpStatusDto dto)
    {
        var res = await _followUpService.UpdateStatus(id, dto.Status);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Follow-up status updated successfully", res));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var res = await _followUpService.DeleteFollowUp(id);
        if (!res)
        {
            return NotFound(new ApiError { Status = StatusCodes.Status404NotFound, Message = "Follow-up not found." });
        }

        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Follow-up deleted successfully", res));
    }
}
