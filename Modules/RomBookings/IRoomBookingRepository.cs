using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.RoomBookings;

public interface IRoomBookingRepository : IRepository<RoomBooking> {
    Task<RoomBooking?> GetBookingByIdWithDetailsAsync(int id);
    Task<IEnumerable<RoomBooking>> GetByUserIdAsync(int userId);
    Task<IEnumerable<RoomBooking>> GetByProviderIdAsync(int providerId, string? status);
}