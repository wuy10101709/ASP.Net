namespace Travel.Models;

public class TourBooking
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public int UserId { get; set; }
    public int NumberOfPeople { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tour Tour { get; set; } = null!;
    public User User { get; set; } = null!;
    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
}