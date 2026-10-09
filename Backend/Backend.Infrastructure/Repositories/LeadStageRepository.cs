using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Infrastructure.Repositories
{
    public class LeadStageRepository: ILeadStageRepositry
    {
        private readonly AppDbContext _context;

        public LeadStageRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get single entity by ID
        public async Task<LeadStage?> GetByIdAsync(int id)
        {
            return await _context.LeadStages.FirstOrDefaultAsync(ls => ls.Id == id);
        }

        // Get all entities
        public async Task<IEnumerable<LeadStage>> GetAllAsync()
        {
            return await _context.LeadStages.AsNoTracking().ToListAsync();
        }

        // Add a new entity
        public async Task AddAsync(LeadStage leadStage)
        {
            await _context.LeadStages.AddAsync(leadStage);
            await _context.SaveChangesAsync();
        }

        // Update an existing entity
        public async Task<LeadStage> UpdateAsync(LeadStage leadStage)
        {
            _context.LeadStages.Update(leadStage);
            await _context.SaveChangesAsync();
            return leadStage;
        }

        // Delete an entity by ID
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }

            _context.LeadStages.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Optional: Check if record exists
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.LeadStages.AnyAsync(ls => ls.Id == id);
        }
    }
}