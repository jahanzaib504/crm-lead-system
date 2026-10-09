using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Domain.Interfaces
{
    public interface ILeadRepository
    {
        // Read operations
        Task<Lead?> GetByIdAsync(int id);
        Task<Lead?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Lead>> GetAllAsync();
        Task<(IEnumerable<Lead> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Lead>> GetByStageIdAsync(int stageId);
        Task<bool> ExistsAsync(int id);

        // Write operations
        Task<Lead> AddAsync(Lead lead);
        Task<Lead> UpdateAsync(Lead lead);
        Task<bool> DeleteAsync(int id);
    }
}