using Backend.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Domain.Interfaces;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(int id);
    Task<IEnumerable<Activity>> GetAllAsync();
    Task<IEnumerable<Activity>> GetByLeadIdAsync(int leadId);
    Task<IEnumerable<Activity>> GetByUserIdAsync(int userId);
    Task<Activity> AddAsync(Activity activity);
    Task<bool> DeleteAsync(int id);
}
