namespace Travel.Models;

public class Room
{
    public int Id { get; set; }
    public int AccommodationId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public int TotalRooms { get; set; }
    public int BookedRooms { get; set; } = 0;
    public string Description { get; set; } = string.Empty;

    public Accommodation Accommodation { get; set; } = null!;
    public ICollection<RoomBooking> RoomBookings { get; set; } = new List<RoomBooking>();
}