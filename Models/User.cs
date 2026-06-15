namespace Travel.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "Tourist"; // Tourist | Provider | Admin
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Provider? Provider { get; set; }
    public ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
    public ICollection<RoomBooking> RoomBookings { get; set; } = new List<RoomBooking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}