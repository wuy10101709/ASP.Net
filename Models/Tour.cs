namespace Travel.Models;

public class Tour
{
    public int Id { get; set; }
    public int ProviderId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public int DurationDays { get; set; }
    public int MaxSlots { get; set; }
    public int BookedSlots { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Provider Provider { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}