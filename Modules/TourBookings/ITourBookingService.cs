using Travel.DTOs;

namespace Travel.Modules.TourBookings;

public interface ITourBookingService
{
    // Tourist đặt tour
    Task<TourBookingResponseDto> BookingTourAsync(
        int userId,
        CreateTourBookingDto dto);

    // Tourist xem lịch sử đặt tour của mình
    Task<IEnumerable<TourBookingResponseDto>> GetMyBookingsAsync(
        int userId, int? tourId);

    // Provider xem danh sách booking của tour mình
    // status: null = tất cả | "Pending" | "Confirmed" | "Cancelled" | "Completed"
    Task<IEnumerable<TourBookingResponseDto>> GetProviderBookingsAsync(
        int userId,
        string? status);

    // Provider xác nhận booking
    Task<(bool Success, string Message)> ConfirmBookingAsync(
        int userId,
        int bookingId);

    // Tourist hoặc Provider hủy booking
    Task<(bool Success, string Message)> CancelBookingAsync(
        int userId,
        int bookingId,
        string userRole);  // ← cần biết role để kiểm tra quyền

    // Provider đánh dấu tour đã hoàn thành
    Task<(bool Success, string Message)> CompleteBookingAsync(
        int userId,
        int bookingId);
}