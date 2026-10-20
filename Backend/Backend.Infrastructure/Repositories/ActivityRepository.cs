using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Infrastructure.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _context;

    public ActivityRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Activity?> GetByIdAsync(int id)
    {
        return await _context.Activities
            .Include(a => a.LoggedByUser)
            .Include(a => a.Lead)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Activity>> GetAllAsync()
    {
        return await _context.Activities
            .Include(a => a.LoggedByUser)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetByLeadIdAsync(int leadId)
    {
        return await _context.Activities
            .Where(a => a.LeadId == leadId)
            .Include(a => a.LoggedByUser)
            .OrderByDescending(a => a.ActivityDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Activity>> GetByUserIdAsync(int userId)
    {
        return await _context.Activities
            .Where(a => a.LoggedByUserId == userId)
            .Include(a => a.Lead)
            .OrderByDescending(a => a.ActivityDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Activity> AddAsync(Activity activity)
    {
        await _context.Activities.AddAsync(activity);
        await _context.SaveChangesAsync();
        return activity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Activities.FindAsync(id);
        if (item == null) return false;
        _context.Activities.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
