using Backend.Application.Dtos.Activity;
using Backend.Application.Dtos.Api;
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
    public async Task<ActionResult<ApiResponse<IEnumerable<ActivityResponseDto>>>> GetByLead(int leadId)
    {
        var res = await _activityService.GetByLeadId(leadId);
        return Ok(new ApiResponse<IEnumerable<ActivityResponseDto>>(StatusCodes.Status200OK, "Activities retrieved successfully", res));
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ActivityResponseDto>>>> GetMyActivities()
    {
        var res = await _activityService.GetMyActivities();
        return Ok(new ApiResponse<IEnumerable<ActivityResponseDto>>(200, "My activities retrieved successfully", res));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ActivityResponseDto>>> Create([FromBody] CreateActivityDto dto)
    {
        var res = await _activityService.CreateActivity(dto);
        var response = new ApiResponse<ActivityResponseDto>(StatusCodes.Status201Created, "Activity created successfully", res);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var res = await _activityService.DeleteActivity(id);
        if (!res)
        {
            return NotFound(new ApiError { Status = StatusCodes.Status404NotFound, Message = "Activity not found." });
        }

        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK, "Activity deleted successfully", res));
    }
}
