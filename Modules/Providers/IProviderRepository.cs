using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.Providers;

public interface IProviderRepository : IRepository<Provider> {
    Task<Provider?> GetByUserIdAsync (int userId);
    Task<IEnumerable<Provider>> GetAllWithUserAsync(bool? isApproved);
}

