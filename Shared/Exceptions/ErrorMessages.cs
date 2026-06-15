namespace Travel.Shared.Exceptions;

public static class ErrorMessages
{
    public const string TourNotFound = "TOUR_NOT_FOUND|Tour không tồn tại.";
    public const string ProviderNotApproved = "PROVIDER_NOT_APPROVED|Tài khoản Provider chưa được Admin duyệt.";
    public const string BookingNotFound = "BOOKING_NOT_FOUND|Đơn đặt tour/phòng không tồn tại.";
    public const string EmailExists = "EMAIL_EXISTS|Email này đã được đăng ký trên hệ thống.";

    public const string InvalidRating = "INVALID_RATING|Số sao đánh giá phải từ 1 đến 5 sao.";
    public const string ProviderNoPermission = "PROVIDER_NO_PERMISSION|Bạn không có quyền chỉnh sửa hoặc tác động đến tài nguyên này.";
    public const string BookingNotCompleted = "BOOKING_NOT_COMPLETED|Chỉ có thể đánh giá chuyến đi sau khi đơn đặt đã hoàn thành.";
    public const string AlreadyReviewed = "ALREADY_REVIEWED|Bạn đã gửi đánh giá cho đơn đặt này rồi, không thể đánh giá tiếp.";

    public const string ProviderNotFound = "PROVIDER_NOT_FOUND|Tài khoản nhà cung cấp không tồn tại.";
    public const string RoomNotFound = "ROOM_NOT_FOUND|Phòng yêu cầu không tồn tại trên hệ thống.";
}