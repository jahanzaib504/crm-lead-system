using Backend.Domain.Interfaces;
using Backend.Domain.Models;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Backend.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.Manager).SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.Manager).SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return await _context.Users.Include(u => u.Role).Include(u => u.Manager).SingleAsync(u => u.Id == user.Id);
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existingUser = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Manager)
                .SingleOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {user.Id} was not found.");
            }

            _context.Entry(existingUser).CurrentValues.SetValues(user);
            existingUser.IsDeleted = false;
            await _context.SaveChangesAsync();

            return existingUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return false;
            }

            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(u => u.Role).Include(u => u.Manager).ToListAsync();
        }
    }
}