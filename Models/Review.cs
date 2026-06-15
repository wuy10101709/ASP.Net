namespace Travel.Models;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TargetType { get; set; } = string.Empty; // Tour | Accommodation
    public int TargetId { get; set; }
    public int? TourBookingId { get; set; }
    public int? RoomBookingId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public TourBooking? TourBooking { get; set; }
    public RoomBooking? RoomBooking { get; set; }
}