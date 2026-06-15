using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;

using Travel.Repositories;
namespace Travel.Modules.Users;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _context.Users
            .AnyAsync(u => u.Email == email);

    public async Task<IEnumerable<User>> GetAllByRoleAsync(string? role)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(role))
            query = query.Where(u => u.Role == role);

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }
}