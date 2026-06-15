using Travel.DTOs;
using Travel.Models;
using Travel.Modules.Rooms;
using Travel.Modules.Providers;

namespace Travel.Modules.RoomBookings;

public class RoomBookingService : IRoomBookingService
{
    private readonly IRoomBookingRepository _roomBookingRepo;
    private readonly IRoomRepository _roomRepo; // Cần dùng để check số lượng phòng trống/giá phòng
    private readonly IProviderRepository _providerRepo; // Cần dùng để check quyền của Provider

    public RoomBookingService(
        IRoomBookingRepository roomBookingRepo,
        IRoomRepository roomRepo,
        IProviderRepository providerRepo)
    {
        _roomBookingRepo = roomBookingRepo;
        _roomRepo = roomRepo;
        _providerRepo = providerRepo;
    }

    // 1. Nghiệp vụ Khách đặt phòng
    public async Task<RoomBookingResponseDto> BookingRoomAsync(int userId, CreateRoomBookingDto dto)
    {
        // - B1: Tìm phòng xem có tồn tại không qua _roomRepo
        var room = await _roomRepo.GetByIdWithDetailsAsync(userId);
        if(room == null) throw new KeyNotFoundException("Phòng không tồn tại");
        // - B2: Kiểm tra phòng còn trống không (Status phòng có phải 'Available' không)
        if (dto.CheckIn >= dto.CheckOut)
            throw new InvalidOperationException(
                "Ngày check-out phải sau ngày check-in.");

        if (dto.CheckIn.Date < DateTime.UtcNow.Date)
            throw new InvalidOperationException(
                "Ngày check-in không được ở quá khứ.");

        int availableRooms = room.TotalRooms - room.BookedRooms;
        if (dto.NumberOfRooms > availableRooms)
            throw new InvalidOperationException(
                $"Không đủ phòng. Chỉ còn {availableRooms} phòng trống.");
        // - B3: Tính toán tổng tiền = Số đêm ở * Giá phòng
        int numberOfNights = (dto.CheckOut - dto.CheckIn).Days;
        decimal totalPrice = numberOfNights * room.PricePerNight * dto.NumberOfRooms;
        // - B4: Tạo Object RoomBooking mới, trạng thái "Pending"
         var booking = new RoomBooking
        {
            RoomId        = dto.RoomId,
            UserId        = userId,
            CheckIn       = dto.CheckIn,
            CheckOut      = dto.CheckOut,
            NumberOfRooms = dto.NumberOfRooms,
            TotalPrice    = totalPrice,
            Status        = "Pending",
            Note          = dto.Note,
            CreatedAt     = DateTime.UtcNow
        };

        room.BookedRooms += dto.NumberOfRooms;
        await _roomRepo.UpdateAsync(room);
        // - B5: Gọi _roomBookingRepo.AddAsync() và SaveChangesAsync()
        await _roomBookingRepo.AddAsync(booking);
        await _roomBookingRepo.SaveChangesAsync();
        // - B6: Map kết quả trả về RoomBookingResponseDto
        var result = await _roomBookingRepo.GetBookingByIdWithDetailsAsync(booking.Id);
        return MapToDto(result!);
    }

    // 2. Nghiệp vụ Khách xem lịch sử đặt phòng của mình
    public async Task<IEnumerable<RoomBookingResponseDto>> GetMyBookingAsync(int userId)
    {
        var bookings = await _roomBookingRepo.GetByUserIdAsync(userId);
        return bookings.Select(b => MapToDto(b));
    }

    // 3. Nghiệp vụ Chủ homestay xem danh sách đặt phòng của cơ sở mình
    public async Task<IEnumerable<RoomBookingResponseDto>> GetProviderBookingAsync(int userId, string? status)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null) return Enumerable.Empty<RoomBookingResponseDto>();

        var bookings = await _roomBookingRepo.GetByProviderIdAsync(provider.Id, status);
        return bookings.Select(b => MapToDto(b));
    }

    // 4. Nghiệp vụ Chủ phòng Duyệt/Xác nhận đơn đặt phòng
    public async Task<(bool Success, string Message)> ConfirmBookingAsync(int userId, int bookingId)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null) return (false, "Không tìm thấy thông tin nhà cung cấp.");

        var booking = await _roomBookingRepo.GetBookingByIdWithDetailsAsync(bookingId);
        if (booking == null) return (false, "Đơn đặt phòng không tồn tại.");

        // Bảo mật: Kiểm tra xem phòng này có đúng là của ông Provider đang gọi API sở hữu không
        if (booking.Room.Accommodation.ProviderId != provider.Id) return (false, "Bạn không có quyền xác nhận đơn này.");
        if (booking.Status != "Pending") return (false, $"Đơn đặt phòng đang ở trạng thái {booking.Status}, không thể xác nhận.");

        booking.Status = "Confimed";
        // Nếu hệ thống của bạn tự động đổi trạng thái phòng thành đã thuê (Booked) thì xử lý ở đây
        
        await _roomBookingRepo.UpdateAsync(booking);
        await _roomBookingRepo.SaveChangesAsync();
        return (true, "Xác nhận đặt phòng thành công.");
    }

    // 5. Nghiệp vụ Hủy phòng (Gộp chung cho cả Khách hủy và Chủ từ chối)
    public async Task<(bool Success, string Message)> CancelBookingAsync(int userId, int bookingId, string userRole)
    {
        var booking = await _roomBookingRepo.GetBookingByIdWithDetailsAsync(bookingId);
        if (booking == null) return (false, "Đơn đặt phòng không tồn tại.");
        if (booking.Status == "Cancelled") return (false, "Đơn đặt phòng này đã bị hủy trước đó.");

        // Phân quyền hủy
        if (userRole == "Tourist")
        {
            if (booking.UserId != userId) return (false, "Bạn không thể hủy đơn đặt phòng của người khác.");
        }
        else if (userRole == "Provider")
        {
            var provider = await _providerRepo.GetByUserIdAsync(userId);
            if (provider == null || booking.Room.Accommodation.ProviderId != provider.Id) 
                return (false, "Bạn không có quyền hủy đơn đặt phòng này.");
        }

        booking.Status = "Cancelled";
        await _roomBookingRepo.UpdateAsync(booking);
        await _roomBookingRepo.SaveChangesAsync();
        return (true, "Hủy đặt phòng thành công.");
    }

    // 6. Nghiệp vụ Đánh dấu Hoàn thành đơn phòng (Khi khách Check-out xong)
    public async Task<(bool Success, string Message)> CompleteBookingAsync(int userId, int bookingId)
    {
        var provider = await _providerRepo.GetByUserIdAsync(userId);
        if (provider == null) return (false, "Không tìm thấy thông tin nhà cung cấp.");

        var booking = await _roomBookingRepo.GetBookingByIdWithDetailsAsync(bookingId);
        if (booking == null) return (false, "Đơn đặt phòng không tồn tại.");
        if (booking.Room.Accommodation.ProviderId != provider.Id) return (false, "Bạn không có quyền xử lý đơn này.");
        if (booking.Status != "Confimed") return (false, "Chỉ có thể hoàn thành những đơn phòng đã được xác nhận.");

        booking.Status = "Completed";
        await _roomBookingRepo.UpdateAsync(booking);
        await _roomBookingRepo.SaveChangesAsync();
        return (true, "Đơn đặt phòng đã hoàn thành xuất sắc.");
    }

    // Hàm phụ giúp Mapping dữ liệu sang DTO sạch sẽ
    private RoomBookingResponseDto MapToDto(RoomBooking rb)
    {
        return new RoomBookingResponseDto
        {
            Id              = rb.Id,
            Status          = rb.Status,
            CheckIn         = rb.CheckIn,
            CheckOut        = rb.CheckOut,
            NumberOfNights  = (rb.CheckOut - rb.CheckIn).Days,
            NumberOfRooms   = rb.NumberOfRooms,
            TotalPrice      = rb.TotalPrice,
            Note            = rb.Note,
            CreatedAt       = rb.CreatedAt,

            // Room info
            RoomId            = rb.RoomId,
            RoomType          = rb.Room?.RoomType ?? "",
            AccommodationName = rb.Room?.Accommodation?.Name ?? "",
            Location          = rb.Room?.Accommodation?.Location ?? "",

            // Tourist info
            TouristName  = rb.User?.FullName ?? "",
            TouristEmail = rb.User?.Email ?? "",
            TouristPhone = rb.User?.Phone ?? ""
        };
    }
}