
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
    AppDbContext _appDbContext;
    public AuditLogRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        return await _appDbContext.AuditLogs.ToListAsync();
    }
}
