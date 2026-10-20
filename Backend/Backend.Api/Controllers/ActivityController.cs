using Backend.Application.Dtos.Activity;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Api.Controllers;

[ApiController]
[Route("activities")]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet("lead/{leadId}")]
    public async Task<ActionResult<IEnumerable<ActivityResponseDto>>> GetByLead(int leadId)
    {
        var res = await _activityService.GetByLeadId(leadId);
        return Ok(res);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<ActivityResponseDto>>> GetMyActivities()
    {
        var res = await _activityService.GetMyActivities();
        return Ok(res);
    }

    [HttpPost]
    public async Task<ActionResult<ActivityResponseDto>> Create([FromBody] CreateActivityDto dto)
    {
        var res = await _activityService.CreateActivity(dto);
        return Ok(res);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var res = await _activityService.DeleteActivity(id);
        return Ok(res);
    }
}
