using Travel.Models;
using Travel.Repositories;
namespace Travel.Modules.Tours;

public interface ITourRepository : IRepository<Tour>
{
    Task<IEnumerable<Tour>> GetAllWithDetailsAsync(
        string? location,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        string? keyword);

    Task<Tour?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Tour>> GetByProviderIdAsync(int providerId);
    Task<bool> HasActiveBookingsAsync(int tourId);
}