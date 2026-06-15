using Travel.Models;

namespace Travel.Modules.Reviews;

public interface IReviewRepository
{
    // Thêm review
    Task AddAsync(Review review);
    Task SaveChangesAsync();

    // Kiểm tra đã review booking này chưa
    Task<bool> HasReviewedTourBookingAsync(int tourBookingId);
    Task<bool> HasReviewedRoomBookingAsync(int roomBookingId);

    // Lấy danh sách review theo Tour
    Task<IEnumerable<Review>> GetByTourIdAsync(int tourId);

    // Lấy danh sách review theo Accommodation
    Task<IEnumerable<Review>> GetByAccommodationIdAsync(int accommodationId);

    // Admin xóa review
    Task<Review?> GetByIdAsync(int id);
    Task DeleteAsync(Review review);
}