using Travel.DTOs;

namespace Travel.Modules.Reviews;

public interface IReviewService
{
    // Tourist review tour sau khi Completed
    Task<ReviewResponseDto> CreateTourReviewAsync(
        int userId, CreateTourReviewDto dto);

    // Tourist review accommodation sau khi Completed
    Task<ReviewResponseDto> CreateAccommodationReviewAsync(
        int userId, CreateAccommodationReviewDto dto);

    // Xem review của tour
    Task<IEnumerable<ReviewResponseDto>> GetTourReviewsAsync(int tourId);

    // Xem review của accommodation
    Task<IEnumerable<ReviewResponseDto>> GetAccommodationReviewsAsync(int accommodationId);

    // Admin xóa review vi phạm
    Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId);
}