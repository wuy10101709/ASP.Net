using Travel.DTOs;
using Travel.Repositories;
using Travel.Models;
using Travel.Modules.Tours;
using Travel.Modules.Users;
using Travel.Modules.Providers;

namespace Travel.Modules.TourBookings;

public class TourBookingService : ITourBookingService
{
    private readonly ITourRepository _tourRepo;
    private readonly ITourBookingRepository _tourbookingRepo;
    private readonly IUserRepository _userRepo;
    private readonly  IProviderRepository _providerRepo;

    public TourBookingService(
        ITourBookingRepository tourbookingRepo,
        ITourRepository tourRepo,
        IUserRepository userRepo,
        IProviderRepository providerRepo)
    {
        _tourRepo = tourRepo;
        _tourbookingRepo = tourbookingRepo;
        _userRepo = userRepo;
        _providerRepo = providerRepo;
    }

    //Đánh dấu Provider hoàn thành Tour
    public async Task<(bool Success, string Message)> CompleteBookingAsync(
        int userId,
        int bookingId){
            var provider = await _providerRepo.GetByUserIdAsync(userId);
            if(provider == null) return (false, "Không tìm thấy nhà cung cấp dịch vụ");

            var booking = await _tourbookingRepo.GetBookingByTourAsync(bookingId);

            if(booking == null) return (false, "Đơn hàng tour không tồn tại");
            if(booking.Tour.ProviderId != provider.Id) return (false, "Bạn không có quyền quản lý tour này");
            if(booking.Status != "Comfimed") return (false,"Chỉ có thể hoàn thành Booking đã xác nhận" );

            booking.Status = "Completed";

            await _tourbookingRepo.UpdateAsync(booking);
            await _tourbookingRepo.SaveChangesAsync();

            return (true, "Đã đánh dấu hoàn thành ");
        }

    //Tourits hoặc Provider có thể hủy booking Tour của riêng mình 
     public async Task<(bool Success, string Message)> CancelBookingAsync(
        int userId,
        int bookingId,
        string userRole){
            var booking = await _tourbookingRepo.GetBookingByTourAsync(bookingId);

            if(booking == null) return (false, "Đơn hàng Booking không tồn tại");
            // Kiểm tra trạng thái đơn hàng 
            if(booking.Status =="Cancelled") return (false, "Đơn hàng đã được hủy ");
            if(booking.Status == "Completed") return(false,"Đơn hàng đã được hoàn thành không được hủy");

            //Lọc Role
            if(userRole =="Provider"){
                var provider = await _providerRepo.GetByUserIdAsync(userId);
                if (provider == null) return (false, "Không tìm thấy nhà cung cấp dịch vụ");
                if(booking.Tour.ProviderId != provider.Id ){
                    return (false,"Bạn không có quyền hủy đơn đặt Tour của nhà cung cấp");
                }
            }else if(userRole == "Tourist"){
                if(booking.UserId != userId){
                    return (false,"Bạn không có quyền hủy đơn hàng của người khác");
                }
            }else{
                return (false,"vai trò người dùng không hợp lệ");
            }

            // Hủy và hoàn trả số chỗ 
            booking.Status = "Cancelled";

            if(booking.Tour != null){
                //Hoàn trả 
                booking.Tour.BookedSlots -= booking.NumberOfPeople;
                await _tourbookingRepo.UpdateAsync(booking);
            }

            await _tourbookingRepo.SaveChangesAsync();
            return (true, "Bạn đã hủy thành công");
        }


    // Provider xác nhận booking
    public async Task<(bool Success, string Message)> ConfirmBookingAsync(
        int userId,
        int bookingId){
            var provider = await _providerRepo.GetByUserIdAsync(userId);
            if(provider == null) return (false, "Không tìm thấy nhà cung cấp dịch vụ");

            var booking = await _tourbookingRepo.GetBookingByTourAsync(bookingId);

            if(booking == null) return (false, "Đơn hàng tour không tồn tại");
            if(booking.Tour.ProviderId != provider.Id) return (false, "Bạn không có quyền quản lý tour này");
            if(booking.Status != "Pending") return (false,$"Không thể xác nhận đơn hàng ở trạng thái: {booking.Status}");

            booking.Status = "Comfimed";

            await _tourbookingRepo.UpdateAsync(booking);
            await _tourbookingRepo.SaveChangesAsync();

            return (true, "Xác nhận đơn đặt Tour thành công");

    }

    // Tourist xem lịch sử đặt tour của mình
    public async Task<IEnumerable<TourBookingResponseDto>> GetMyBookingsAsync(
        int userId, int? tourId){
            var tourits = await _userRepo.GetByIdAsync(userId);
            if(tourits == null) throw new KeyNotFoundException("Không tìm thấy thông tin khách hàng");

            var bookings = await _tourbookingRepo.GetCustomerHistoryWithFilterAsync(tourits.Id, tourId);
            return bookings.Select(MapToDto);
        }   

    // Provider xem danh sách booking của tour mình
    // status: null = tất cả | "Pending" | "Confirmed" | "Cancelled" | "Completed"
    public async Task<IEnumerable<TourBookingResponseDto>> GetProviderBookingsAsync(int userId,string? status){
            var provider = await _providerRepo.GetByUserIdAsync(userId);
            if (provider == null) 
                throw new KeyNotFoundException("Không tìm thấy thông tin nhà cung cấp.");

            var bookings = await _tourbookingRepo.GetBookingsByProviderAsync(provider.Id, status);

            return bookings.Select(MapToDto);
        }


    public async Task<TourBookingResponseDto> BookingTourAsync(
        int userId, CreateTourBookingDto dto)
    {
        // 1. Kiểm tra tour tồn tại
        var tour = await _tourRepo.GetByIdAsync(dto.TourId); // ← fix TourId
        if (tour == null)
            throw new KeyNotFoundException("Tour không tồn tại.");

        // 2. Kiểm tra số chỗ còn trống — chỉ 1 lần
        int availableSlots = tour.MaxSlots - tour.BookedSlots;
        if (dto.NumberOfPeople > availableSlots)
            throw new InvalidOperationException(
                $"Không đủ chỗ. Chỉ còn {availableSlots} chỗ trống.");

        // 3. Kiểm tra ngày khởi hành chưa qua
        if (tour.DepartureDate > DateTime.UtcNow)
            throw new InvalidOperationException("Tour này đã khởi hành rồi.");

        // 4. Kiểm tra user chưa đặt tour này
        var existed = await _tourbookingRepo
            .HasUserBookedTourAsync(userId, dto.TourId);
        if (existed)
            throw new InvalidOperationException(
                "Bạn đã đặt tour này rồi. Mỗi tài khoản chỉ được đặt một lần.");

        // 5. Tính tổng tiền
        decimal totalPrice = tour.Price * dto.NumberOfPeople;

        // 6. Tạo booking
        var booking = new TourBooking
        {
            TourId         = dto.TourId,
            UserId         = userId,
            NumberOfPeople = dto.NumberOfPeople,
            TotalPrice     = totalPrice,
            Status         = "Pending",
            Note           = dto.Note,
            CreatedAt      = DateTime.UtcNow
        };

        // 7. Cập nhật số chỗ đã đặt
        tour.BookedSlots += dto.NumberOfPeople; // ← fix: dto thay vì request
        await _tourRepo.UpdateAsync(tour);      // ← fix: UpdateAsync

        // 8. Lưu booking
        await _tourbookingRepo.AddAsync(booking);
        await _tourbookingRepo.SaveChangesAsync();

        // 9. Nạp dữ liệu chi tiết 1 lần — trả về DTO
        var completedBooking = await _tourbookingRepo
            .GetBookingIdWithDetailsAsync(booking.Id);

        return MapToDto(completedBooking!); // ← fix: thêm return
    }

    // ── Helper Map ────────────────────────────────────────
    private static TourBookingResponseDto MapToDto(TourBooking t) => new()
    {
        Id             = t.Id,
        Status         = t.Status,
        NumberOfPeople = t.NumberOfPeople,
        TotalPrice     = t.TotalPrice,
        Note           = t.Note,
        CreatedAt      = t.CreatedAt, // ← fix: CreatedAt

        // Tour info — qua Navigation Property
        TourId        = t.TourId,
        TourName      = t.Tour?.Name ?? "",          // ← fix
        TourLocation  = t.Tour?.Location ?? "",      // ← fix
        DepartureDate = t.Tour?.DepartureDate        // ← fix
                        ?? DateTime.MinValue,

        // Tourist info — qua Navigation Property
        TouristName  = t.User?.FullName ?? "", // ← fix
        TouristEmail = t.User?.Email ?? "",    // ← fix
        TouristPhone = t.User?.Phone ?? ""     // ← fix
    };
}