using Travel.DTOs;
using Travel.Models;
using Travel.Data;
using Travel.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Travel.Modules.Reviews;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepo;
    private readonly AppDbContext _context;

    public ReviewService(IReviewRepository reviewRepo, AppDbContext context)
    {
        _reviewRepo = reviewRepo;
        _context = context;
    }

    // ── Review Tour ───────────────────────────────────────
    public async Task<ReviewResponseDto> CreateTourReviewAsync(
        int userId, CreateTourReviewDto dto)
    {
        // B1: Kiểm tra rating hợp lệ
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new BadRequestException(
                ErrorMessages.InvalidRating);

        // B2: Tìm TourBooking
        var booking = await _context.TourBookings
            .Include(tb => tb.Tour)
            .FirstOrDefaultAsync(tb => tb.Id == dto.TourBookingId)
            ?? throw new NotFoundException(
                ErrorMessages.BookingNotFound);

        // B3: Kiểm tra booking thuộc về user này
        if (booking.UserId != userId)
            throw new ForbiddenException(
                ErrorMessages.ProviderNoPermission);

        // B4: Kiểm tra booking đã Completed chưa
        if (booking.Status != "Completed")
            throw new BadRequestException(
                ErrorMessages.BookingNotCompleted);

        // B5: Kiểm tra đã review booking này chưa
        var alreadyReviewed = await _reviewRepo
            .HasReviewedTourBookingAsync(dto.TourBookingId);
        if (alreadyReviewed)
            throw new BadRequestException(
                ErrorMessages.AlreadyReviewed);

        // B6: Tạo Review
        var review = new Review
        {
            UserId        = userId,
            TargetType    = "Tour",
            TargetId      = booking.TourId,
            TourBookingId = dto.TourBookingId,
            RoomBookingId = null,
            Rating        = dto.Rating,
            Comment       = dto.Comment,
            CreatedAt     = DateTime.UtcNow
        };

        await _reviewRepo.AddAsync(review);
        await _reviewRepo.SaveChangesAsync();

        // B7: Trả về DTO
        var user = await _context.Users.FindAsync(userId);

        return new ReviewResponseDto
        {
            Id           = review.Id,
            Rating       = review.Rating,
            Comment      = review.Comment,
            CreatedAt    = review.CreatedAt,
            ReviewerName = user?.FullName ?? "",
            TargetType   = "Tour",
            TargetName   = booking.Tour?.Name ?? ""
        };
    }

    // ── Review Accommodation ──────────────────────────────
    public async Task<ReviewResponseDto> CreateAccommodationReviewAsync(
        int userId, CreateAccommodationReviewDto dto)
    {
        // B1: Kiểm tra rating hợp lệ
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new BadRequestException(
                ErrorMessages.InvalidRating);

        // B2: Tìm RoomBooking
        var booking = await _context.RoomBookings
            .Include(rb => rb.Room)
                .ThenInclude(r => r.Accommodation)
            .FirstOrDefaultAsync(rb => rb.Id == dto.RoomBookingId)
            ?? throw new NotFoundException(
                ErrorMessages.BookingNotFound);

        // B3: Kiểm tra booking thuộc về user này
        if (booking.UserId != userId)
            throw new ForbiddenException(
                ErrorMessages.ProviderNoPermission);

        // B4: Kiểm tra booking đã Completed chưa
        if (booking.Status != "Completed")
            throw new BadRequestException(
                ErrorMessages.BookingNotCompleted);

        // B5: Kiểm tra đã review booking này chưa
        var alreadyReviewed = await _reviewRepo
            .HasReviewedRoomBookingAsync(dto.RoomBookingId);
        if (alreadyReviewed)
            throw new BadRequestException(
                ErrorMessages.AlreadyReviewed);

        // B6: Tạo Review
        var review = new Review
        {
            UserId        = userId,
            TargetType    = "Accommodation",
            TargetId      = booking.Room.AccommodationId,
            TourBookingId = null,
            RoomBookingId = dto.RoomBookingId,
            Rating        = dto.Rating,
            Comment       = dto.Comment,
            CreatedAt     = DateTime.UtcNow
        };

        await _reviewRepo.AddAsync(review);
        await _reviewRepo.SaveChangesAsync();

        // B7: Trả về DTO
        var user = await _context.Users.FindAsync(userId);

        return new ReviewResponseDto
        {
            Id           = review.Id,
            Rating       = review.Rating,
            Comment      = review.Comment,
            CreatedAt    = review.CreatedAt,
            ReviewerName = user?.FullName ?? "",
            TargetType   = "Accommodation",
            TargetName   = booking.Room?.Accommodation?.Name ?? ""
        };
    }

    // ── Xem review của Tour ───────────────────────────────
    public async Task<IEnumerable<ReviewResponseDto>> GetTourReviewsAsync(int tourId)
    {
        var reviews = await _reviewRepo.GetByTourIdAsync(tourId);

        return reviews.Select(r => new ReviewResponseDto
        {
            Id           = r.Id,
            Rating       = r.Rating,
            Comment      = r.Comment,
            CreatedAt    = r.CreatedAt,
            ReviewerName = r.User?.FullName ?? "",
            TargetType   = "Tour",
            TargetName   = r.TourBooking?.Tour?.Name ?? ""
        });
    }

    // ── Xem review của Accommodation ─────────────────────
    public async Task<IEnumerable<ReviewResponseDto>> GetAccommodationReviewsAsync(
        int accommodationId)
    {
        var reviews = await _reviewRepo
            .GetByAccommodationIdAsync(accommodationId);

        return reviews.Select(r => new ReviewResponseDto
        {
            Id           = r.Id,
            Rating       = r.Rating,
            Comment      = r.Comment,
            CreatedAt    = r.CreatedAt,
            ReviewerName = r.User?.FullName ?? "",
            TargetType   = "Accommodation",
            TargetName   = r.RoomBooking?.Room?.Accommodation?.Name ?? ""
        });
    }

    // ── Admin xóa review ──────────────────────────────────
    public async Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId)
    {
        var review = await _reviewRepo.GetByIdAsync(reviewId);

        if (review == null)
            return (false, "Review không tồn tại.");

        await _reviewRepo.DeleteAsync(review);
        await _reviewRepo.SaveChangesAsync();

        return (true, "Đã xóa review thành công.");
    }
}