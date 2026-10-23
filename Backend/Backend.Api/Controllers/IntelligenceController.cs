using Backend.Application.Dtos.Common;
using Backend.Application.Dtos.Lead;
using Backend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("intelligence")]
public class IntelligenceController
{
    private readonly IIntelligenceService _intelligenceService;

    public IntelligenceController(IIntelligenceService intelligenceService)
    {
        _intelligenceService = intelligenceService ?? throw new ArgumentNullException(nameof(intelligenceService));
    }

    [HttpGet("recommendations")]
    public async Task<PagedResponseDto<LeadResponseDto>> GetRecommendations([FromQuery] int page, [FromQuery] int pageSize)
    {
        var leads = await _intelligenceService.GetRecommendations(page, pageSize);
        return leads;
    }
}
