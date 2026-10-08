using Backend.Domain.Models;

namespace Backend.Domain.Interfaces;
public interface ICompanyRepositry
{
    Task<Company?> GetByIdAsync(int id);
    Task<Company?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Company>> GetAllAsync();
    Task<(IEnumerable<Company> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Company>> GetByStageIdAsync(int stageId);
    Task<bool> ExistsAsync(int id);

    // Write operations
    Task<Company> AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task<bool> DeleteAsync(int id);
}

