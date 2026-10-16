using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly AppDbContext _context; 

    public NoteRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        return await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id);
    }
    public async Task<Note?> GetByIdWithLeadAsync(int id)
    {
        return await _context.Notes.Include(n => n.Lead).FirstOrDefaultAsync(n => n.Id == id);
    }
    public async Task<IEnumerable<Note>> GetAllAsync()
    {
        return await _context.Notes
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IEnumerable<Note>> GetAllWithLeadsAsync()
    {
        return await _context.Notes.Include(n=>n.Lead)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<(IEnumerable<Note> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Notes.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Notes.AnyAsync(n => n.Id == id);
    }

    public async Task<Note> AddAsync(Note note)
    {
        if (note == null) throw new ArgumentNullException(nameof(note));

        await _context.Notes.AddAsync(note);
        await _context.SaveChangesAsync();

        return note;
    }

    public async Task<Note> UpdateAsync(Note note)
    {
        if (note == null) throw new ArgumentNullException(nameof(note));

        _context.Notes.Update(note);
        await _context.SaveChangesAsync();

        return note;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null)
        {
            return false;
        }

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return true;
    }
}