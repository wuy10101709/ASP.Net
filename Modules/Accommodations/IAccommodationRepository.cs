using Travel.Models;
using Travel.DTOs;
using Travel.Repositories;

namespace Travel.Modules.Accommodations;

public interface IAccommodationRepository : IRepository<Accommodation> {

        Task<IEnumerable<Accommodation>> GetAccommodations(
            string? location,
            int? categoryId,
            float? minStar,
             string? keyword);

        Task<Accommodation?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Accommodation>> GetByProviderIdAsync(int providerId);
        Task<bool> HasActiveBookingsAsync(int accomodationId);
}