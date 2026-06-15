using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.Categorys;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetByTypeAsync(string type)
    {
        return await _context.Categories
            .Where(c => c.Type == type)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> IsValidCategoryAsync(int id, string type)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id && c.Type == type);
    }
}