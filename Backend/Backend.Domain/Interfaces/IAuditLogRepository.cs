using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task<IEnumerable<AuditLog>> GetAllAsync();
    Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
}
