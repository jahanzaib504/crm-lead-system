using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Api;
using Backend.Application.Dtos.Lead;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("intelligence")]
[Authorize(Roles = "Admin,Sales Manager")]
public class IntelligenceController : ControllerBase
{
    private readonly IIntelligenceService _intelligenceService;

    public IntelligenceController(IIntelligenceService intelligenceService)
    {
        _intelligenceService = intelligenceService ?? throw new ArgumentNullException(nameof(intelligenceService));
    }

    [HttpGet("recommendations")]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<LeadResponseDto>>>> GetRecommendations([FromQuery] int page, [FromQuery] int pageSize)
    {
        var leads = await _intelligenceService.GetRecommendations(page, pageSize);
        return Ok(new ApiResponse<PagedResponseDto<LeadResponseDto>>(StatusCodes.Status200OK, "Recommendations retrieved successfully", leads));
    }
}
