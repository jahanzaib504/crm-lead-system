
using System;
using System.Collections.Generic;
using System.Text;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Backend.Infrastructure.Repositories;

public class AuditLogRepository: IAuditLogRepository
{
    private readonly AppDbContext _appDbContext;

    public AuditLogRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        return await _appDbContext.AuditLogs.ToListAsync();
    }

    public async Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _appDbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);
        return auditLog;
    }
}
