using Backend.Application.Dtos.AuditLog;
using Backend.Application.Dtos.Common;
using Backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers;

[ApiController]
[Route("/audit-logs")]
public class AuditLogController: Controller
{
    IAuditLogService _auditLogService;
    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<PagedResponseDto<AuditLogResponseDto>> GetAuditLogsPaged([FromQuery] AuditLogRequestDto requestDto)
    {
        return await _auditLogService.GetAuditLogsPaged(requestDto);
    }
}
