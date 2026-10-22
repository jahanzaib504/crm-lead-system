using Backend.Application.Dtos.AuditLog;
using Backend.Application.Dtos.Common;
using Backend.Domain.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Application.Services;
public interface IAuditLogService
{
    public Task<PagedResponseDto<AuditLogResponseDto>> GetAuditLogsPaged(AuditLogRequestDto dto);
}
public class AuditLogService: IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepo;
    public AuditLogService(IAuditLogRepository auditLogRepo)
    {
        _auditLogRepo = auditLogRepo;
    }
    public async Task<PagedResponseDto<AuditLogResponseDto>> GetAuditLogsPaged(AuditLogRequestDto dto)
    {
        var logs = await _auditLogRepo.GetAllAsync();
        var totalCount = logs.Count();
        var pagedLogs = logs.Skip(dto.page - 1).Take(dto.pageSize).ToList();
        return new PagedResponseDto<AuditLogResponseDto> { Items = pagedLogs.Adapt<AuditLogResponseDto[]>(), TotalCount = totalCount };
    }
}
