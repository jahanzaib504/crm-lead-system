using Backend.Application.Dtos.AuditLog;
using Backend.Application.Dtos.Api;
using Backend.Application.Dtos.Common;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("/audit-logs")]
public class AuditLogController: ControllerBase
{
    private readonly IAuditLogService _auditLogService;
    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<AuditLogResponseDto>>>> GetAuditLogsPaged([FromQuery] AuditLogRequestDto requestDto)
    {
        var result = await _auditLogService.GetAuditLogsPaged(requestDto);
        return Ok(new ApiResponse<PagedResponseDto<AuditLogResponseDto>>(StatusCodes.Status200OK, "Audit logs retrieved successfully", result));
    }
}
