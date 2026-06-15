using Travel.Models;
using Travel.DTOs;

namespace Travel.Modules.RoomBookings;

public interface IRoomBookingService {
    Task<RoomBookingResponseDto> BookingRoomAsync(int userId, CreateRoomBookingDto dto);
    Task<IEnumerable<RoomBookingResponseDto>> GetMyBookingAsync(int userId);
    Task<IEnumerable<RoomBookingResponseDto>> GetProviderBookingAsync(int userId, string? status);
    Task<(bool Success, string Message)> ConfirmBookingAsync(int userId, int bookingId);
    Task<(bool Success, string Message)> CancelBookingAsync(int userId, int bookingId, string userRole);
    Task<(bool Success, string Message)> CompleteBookingAsync(int userId, int bookingId);
}