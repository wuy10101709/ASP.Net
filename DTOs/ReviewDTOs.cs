namespace Travel.DTOs;

// ── Request DTOs ──────────────────────────────────────

public class CreateTourReviewDto
{
    public int TourBookingId { get; set; }  // Booking đã Completed
    public int Rating { get; set; }          // 1 - 5
    public string Comment { get; set; } = string.Empty;
}

public class CreateAccommodationReviewDto
{
    public int RoomBookingId { get; set; }  // Booking đã Completed
    public int Rating { get; set; }          // 1 - 5
    public string Comment { get; set; } = string.Empty;
}

// ── Response DTO ──────────────────────────────────────

public class ReviewResponseDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Người review
    public string ReviewerName { get; set; } = string.Empty;

    // Tour hoặc Accommodation
    public string TargetType { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
}