using Backend.Application.Dtos.AuditLog;
using Backend.Application.Dtos.Common;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Mapster;
using System.Text.Json;

namespace Backend.Application.Services;
public interface IAuditLogService
{
    Task<PagedResponseDto<AuditLogResponseDto>> GetAuditLogsPaged(AuditLogRequestDto dto);
}
public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepo;

    public AuditLogService(IAuditLogRepository auditLogRepo)
    {
        _auditLogRepo = auditLogRepo;
    }

    internal async Task LogAsync(
        string entityName,
        int entityId,
        string action,
        object changes,
        int? performedByUserId,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            ChangesJson = JsonSerializer.Serialize(changes),
            PerformedByUserId = performedByUserId,
            Timestamp = DateTime.UtcNow
        };

        await _auditLogRepo.AddAsync(auditLog, cancellationToken);
    }

    public async Task<PagedResponseDto<AuditLogResponseDto>> GetAuditLogsPaged(AuditLogRequestDto dto)
    {
        var logs = await _auditLogRepo.GetAllAsync();
        var logList = logs.ToList();
        var totalCount = logList.Count;
        var pagedLogs = logList.OrderByDescending(l => l.Timestamp).Skip((dto.page - 1) * dto.pageSize).Take(dto.pageSize).ToList();
        return new PagedResponseDto<AuditLogResponseDto> { Items = pagedLogs.Adapt<AuditLogResponseDto[]>(), TotalCount = totalCount };
    }
}
