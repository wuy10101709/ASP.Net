using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.Admin;

public interface IAdminRepository
{
    // Provider
    Task<IEnumerable<Provider>> GetAllProvidersAsync(bool? isApproved);
    Task<Provider?> GetProviderByIdAsync(int id);

    // User
    Task<IEnumerable<User>> GetAllUsersAsync(string? role);
    Task<User?> GetUserByIdAsync(int id);
    Task DeleteUserAsync(User user);

    // Tour
    Task<IEnumerable<Tour>> GetAllToursAsync();
    Task<Tour?> GetTourByIdAsync(int id);
    Task DeleteTourAsync(Tour tour);

    // Booking
    Task<IEnumerable<TourBooking>> GetAllTourBookingsAsync(string? status);

    // Stats
    Task<int> CountUsersAsync();
    Task<int> CountProvidersAsync();
    Task<int> CountApprovedProvidersAsync();
    Task<int> CountPendingProvidersAsync();
    Task<int> CountToursAsync();
    Task<int> CountTourBookingsAsync();
    Task<int> CountRoomBookingsAsync();
    Task<decimal> GetTotalRevenueAsync();

    Task SaveChangesAsync();
}