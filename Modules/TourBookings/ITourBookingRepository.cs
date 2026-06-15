using Travel.Models;
using Travel.Repositories;

namespace Travel.Modules.TourBookings;

public interface ITourBookingRepository : IRepository<TourBooking> {
    // Kiểm tra xem User đã có đơn đặt tour này với trạng thái hợp lệ chưa
    Task<bool> HasUserBookedTourAsync(int userId, int tourId);
    Task<TourBooking?> GetBookingIdWithDetailsAsync(int bookingId);
   // Thêm hàm lọc danh sách booking thuộc về các tour của một Provider cụ thể
    Task<IEnumerable<TourBooking>> GetBookingsByProviderAsync(int providerId, string? status);
    // Hàm lấy lịch sử của riêng 1 khách hàng, có thể lọc theo TourId nếu muốn
    Task<IEnumerable<TourBooking>> GetCustomerHistoryWithFilterAsync(int userId, int? tourId);
    //Lấy thông tin đơn đặt từ TourBooking kèm theo bảng Tour kiểm tra 
    Task<TourBooking?> GetBookingByTourAsync(int id);
}