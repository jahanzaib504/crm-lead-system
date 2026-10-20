using Backend.Application.Dtos.Analytics;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.Api.Controllers;

[ApiController]
[Route("analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<AnalyticsResponseDto>> GetDashboardAnalytics()
    {
        var result = await _analyticsService.GetDashboardAnalytics();
        return Ok(result);
    }
}
