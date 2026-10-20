using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Infrastructure.Repositories;

public class FollowUpRepository : IFollowUpRepository
{
    private readonly AppDbContext _context;

    public FollowUpRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FollowUp?> GetByIdAsync(int id)
    {
        return await _context.FollowUps
            .Include(f => f.AssignedToUser)
            .Include(f => f.Lead)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<FollowUp>> GetAllAsync()
    {
        return await _context.FollowUps
            .Include(f => f.AssignedToUser)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<FollowUp>> GetByLeadIdAsync(int leadId)
    {
        return await _context.FollowUps
            .Where(f => f.LeadId == leadId)
            .Include(f => f.AssignedToUser)
            .OrderBy(f => f.DueDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<FollowUp>> GetByUserIdAsync(int userId)
    {
        return await _context.FollowUps
            .Where(f => f.AssignedToUserId == userId)
            .Include(f => f.Lead)
            .OrderBy(f => f.DueDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FollowUp> AddAsync(FollowUp followUp)
    {
        await _context.FollowUps.AddAsync(followUp);
        await _context.SaveChangesAsync();
        return followUp;
    }

    public async Task<FollowUp> UpdateAsync(FollowUp followUp)
    {
        _context.FollowUps.Update(followUp);
        await _context.SaveChangesAsync();
        return followUp;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.FollowUps.FindAsync(id);
        if (item == null) return false;
        _context.FollowUps.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
