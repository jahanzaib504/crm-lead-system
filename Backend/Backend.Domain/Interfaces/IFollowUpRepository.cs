using Backend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Domain.Interfaces;

public interface IFollowUpRepository
{
    Task<FollowUp?> GetByIdAsync(int id);
    Task<IEnumerable<FollowUp>> GetAllAsync();
    Task<IEnumerable<FollowUp>> GetByLeadIdAsync(int leadId);
    Task<IEnumerable<FollowUp>> GetByUserIdAsync(int userId);
    Task<FollowUp> AddAsync(FollowUp followUp);
    Task<FollowUp> UpdateAsync(FollowUp followUp);
    Task<bool> DeleteAsync(int id);
}
