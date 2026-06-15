using Microsoft.EntityFrameworkCore;
using Travel.Models;

namespace Travel.Data;

public class AppDbContext : DbContext
{
    // Nhận địa chỉ và phiên bản đóng gói đưa lên lớp cha (DbContext)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<Accommodation> Accommodations { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<TourBooking> TourBookings { get; set; }
    public DbSet<RoomBooking> RoomBookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }

    // Phương thức này để mapping 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User → Provider (1-1)
        modelBuilder.Entity<Provider>()
            .HasOne(p => p.User)
            .WithOne(u => u.Provider)
            .HasForeignKey<Provider>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Provider → Tours / Accommodations
        modelBuilder.Entity<Tour>()
            .HasOne(t => t.Provider)
            .WithMany(p => p.Tours)
            .HasForeignKey(t => t.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Accommodation>()
            .HasOne(a => a.Provider)
            .WithMany(p => p.Accommodations)
            .HasForeignKey(a => a.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Category → Tours / Accommodations
        modelBuilder.Entity<Tour>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Tours)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Accommodation>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Accommodations)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Accommodation → Rooms
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Accommodation)
            .WithMany(a => a.Rooms)
            .HasForeignKey(r => r.AccommodationId)
            .OnDelete(DeleteBehavior.Cascade);

        // TourBooking
        modelBuilder.Entity<TourBooking>()
            .HasOne(tb => tb.Tour)
            .WithMany(t => t.TourBookings)
            .HasForeignKey(tb => tb.TourId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TourBooking>()
            .HasOne(tb => tb.User)
            .WithMany(u => u.TourBookings)
            .HasForeignKey(tb => tb.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // RoomBooking
        modelBuilder.Entity<RoomBooking>()
            .HasOne(rb => rb.Room)
            .WithMany(r => r.RoomBookings)
            .HasForeignKey(rb => rb.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoomBooking>()
            .HasOne(rb => rb.User)
            .WithMany(u => u.RoomBookings)
            .HasForeignKey(rb => rb.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Payment → TourBooking (1-1)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.TourBooking)
            .WithOne(tb => tb.Payment)
            .HasForeignKey<Payment>(p => p.TourBookingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Payment → RoomBooking (1-1)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.RoomBooking)
            .WithOne(rb => rb.Payment)
            .HasForeignKey<Payment>(p => p.RoomBookingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Payment → User
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Review → User
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Review → TourBooking (1-1)
        modelBuilder.Entity<Review>()
            .HasOne(r => r.TourBooking)
            .WithOne(tb => tb.Review)
            .HasForeignKey<Review>(r => r.TourBookingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Review → RoomBooking (1-1)
        modelBuilder.Entity<Review>()
            .HasOne(r => r.RoomBooking)
            .WithOne(rb => rb.Review)
            .HasForeignKey<Review>(r => r.RoomBookingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Decimal precision
        modelBuilder.Entity<Tour>().Property(t => t.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Room>().Property(r => r.PricePerNight).HasPrecision(18, 2);
        modelBuilder.Entity<TourBooking>().Property(tb => tb.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<RoomBooking>().Property(rb => rb.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Biển & đảo", Type = "Tour", Description = "Tour khám phá biển đảo Quy Nhơn, Phú Yên" },
            new Category { Id = 2, Name = "Núi & thác", Type = "Tour", Description = "Trekking, khám phá thiên nhiên" },
            new Category { Id = 3, Name = "Văn hóa & di tích", Type = "Tour", Description = "Tháp Chăm, phố cổ, bảo tàng" },
            new Category { Id = 4, Name = "Ẩm thực", Type = "Tour", Description = "Tour trải nghiệm ẩm thực địa phương" },
            new Category { Id = 5, Name = "Khám phá", Type = "Tour", Description = "Tour tổng hợp nhiều điểm đến" },
            new Category { Id = 6, Name = "Khách sạn", Type = "Accommodation", Description = "Khách sạn từ 1-5 sao" },
            new Category { Id = 7, Name = "Homestay", Type = "Accommodation", Description = "Nhà dân, trải nghiệm địa phương" },
            new Category { Id = 8, Name = "Resort", Type = "Accommodation", Description = "Khu nghỉ dưỡng cao cấp" },
            new Category { Id = 9, Name = "Villa", Type = "Accommodation", Description = "Biệt thự cho thuê nguyên căn" }
        );
    }
}