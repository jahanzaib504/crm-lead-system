using Backend.Application.Dtos.FollowUp;
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
    public async Task<ActionResult<IEnumerable<FollowUpResponseDto>>> GetByLead(int leadId)
    {
        var res = await _followUpService.GetByLeadId(leadId);
        return Ok(res);
    }

    [HttpGet("my-pending")]
    public async Task<ActionResult<IEnumerable<FollowUpResponseDto>>> GetMyPending()
    {
        var res = await _followUpService.GetMyPendingFollowUps();
        return Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<FollowUpResponseDto>> Create([FromBody] CreateFollowUpDto dto)
    {
        var res = await _followUpService.CreateFollowUp(dto);
        return Ok(res);
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<bool>> UpdateStatus(int id, [FromBody] UpdateFollowUpStatusDto dto)
    {
        var res = await _followUpService.UpdateStatus(id, dto.Status);
        return Ok(res);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var res = await _followUpService.DeleteFollowUp(id);
        return Ok(res);
    }
}
