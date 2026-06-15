namespace Travel.Models;

public class Payment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string BookingType { get; set; } = string.Empty; // Tour | Room
    public int? TourBookingId { get; set; }
    public int? RoomBookingId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "VNPay";
    public string Status { get; set; } = "Pending";
    public string TransactionId { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public TourBooking? TourBooking { get; set; }
    public RoomBooking? RoomBooking { get; set; }
}