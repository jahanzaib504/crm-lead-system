using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Interfaces;

namespace Backend.Infrastructure.Repositories
{
    public class LeadRepository: ILeadRepository
    {
        private readonly AppDbContext _context;

        public LeadRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get single Lead by ID
        public async Task<Lead?> GetByIdAsync(int id)
        {
            return await _context.Leads
                .Include(l => l.LeadStage)
                .Include(l => l.Contact)
                .Include(l => l.Company)
                .Include(l => l.AssignedToUser)
                .Include(l => l.Manager)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        // Get all Leads (read-only optimized)
        public async Task<IEnumerable<Lead>> GetAllAsync()
        {
            return await _context.Leads
                .Include(l => l.LeadStage)
                .Include(l => l.Contact)
                .Include(l => l.Company)
                .Include(l => l.AssignedToUser)
                .Include(l => l.Manager)
                .AsNoTracking()
                .ToListAsync();
        }

        // Get paged list of Leads
        public async Task<(IEnumerable<Lead> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Leads.AsNoTracking();
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(l => l.LeadStage)
                .Include(l => l.Contact)
                .Include(l => l.Company)
                .Include(l => l.AssignedToUser)
                .Include(l => l.Manager)
                .ToListAsync();

            return (items, totalCount);
        }

        // Get Leads by Stage ID
        public async Task<IEnumerable<Lead>> GetByStageIdAsync(int stageId)
        {
            return await _context.Leads
                .AsNoTracking()
                .Where(l => l.LeadStageId == stageId)
                .Include(_l => _l.LeadStage)
                .ToListAsync();
        }

        // Add a new Lead
        public async Task<Lead> AddAsync(Lead lead)
        {
            if (lead == null) throw new ArgumentNullException(nameof(lead));

            await _context.Leads.AddAsync(lead);
            await _context.SaveChangesAsync();
            return lead;
        }

        // Update an existing Lead
        public async Task<Lead> UpdateAsync(Lead lead)
        {
            if (lead == null) throw new ArgumentNullException(nameof(lead));

            var existingLead = await _context.Leads
                .FirstOrDefaultAsync(l => l.Id == lead.Id);
            if (existingLead == null)
            {
                throw new KeyNotFoundException($"Lead with ID {lead.Id} was not found.");
            }

            _context.Entry(existingLead).CurrentValues.SetValues(lead);
            existingLead.IsDeleted = false;
            await _context.SaveChangesAsync();
            return existingLead;
        }

        // Delete a Lead by ID
        public async Task<bool> DeleteAsync(int id)
        {
            var lead = await _context.Leads.FirstOrDefaultAsync(l => l.Id == id);
            if (lead == null)
            {
                return false;
            }

            lead.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if Lead exists
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Leads.AnyAsync(l => l.Id == id);
        }
    }
}