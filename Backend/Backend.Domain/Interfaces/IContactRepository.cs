using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces
{
    public interface IContactRepository
    {
        Task<Contact?> GetByIdAsync(int id);
        Task<Contact?> GetByIdWithLeadsAsync(int id);
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<(IEnumerable<Contact> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> ExistsAsync(int id);

        // Write operations
        Task<Contact> AddAsync(Contact contact);
        Task<Contact> UpdateAsync(Contact contact);
        Task<bool> DeleteAsync(int id);
    }
}
