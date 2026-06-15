using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.Providers;

public class ProviderRepository : BaseRepository<Provider>, IProviderRepository
{
    public ProviderRepository(AppDbContext context) : base(context) { }

    public async Task<Provider?> GetByUserIdAsync(int userId) =>
        await _context.Providers
            .FirstOrDefaultAsync(p => p.UserId == userId);

    public async Task<IEnumerable<Provider>> GetAllWithUserAsync(bool? isApproved)
    {
        var query = _context.Providers
            .Include(p => p.User)
            .AsQueryable();

        if (isApproved.HasValue)
            query = query.Where(p => p.IsApproved == isApproved);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}