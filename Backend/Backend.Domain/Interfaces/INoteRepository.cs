using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces;
public interface INoteRepository
{
    Task<Note?> GetByIdAsync(int id);
    Task<IEnumerable<Note>> GetAllAsync();
    Task<(IEnumerable<Note> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<bool> ExistsAsync(int id);

    // Write operations
    Task<Note> AddAsync(Note note);
    Task UpdateAsync(Note note);
    Task<bool> DeleteAsync(int id);
}

