using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel.DTOs;
using Travel.Modules.TourBookings;

[Route("api/tour-bookings")]
[ApiController]
[Authorize] // Tất cả các hàm trong này đều yêu cầu đăng nhập
public class TourBookingsController : ControllerBase
{
    private readonly ITourBookingService _tourBookingService;

    public TourBookingsController(ITourBookingService tourBooking)
    {
        _tourBookingService = tourBooking;
    }

    // ── POST /api/tour-bookings ───────────────────────────
    [HttpPost]
    [Authorize(Roles = "Tourist")]
    public async Task<ActionResult<TourBookingResponseDto>> CreateBooking(CreateTourBookingDto request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var tourBooking = await _tourBookingService.BookingTourAsync(userId, request);
            return Ok(tourBooking);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ── GET /api/tour-bookings/my-bookings ────────────────
    [HttpGet("my-bookings")]
    [Authorize(Roles = "Tourist")]
    public async Task<ActionResult<IEnumerable<TourBookingResponseDto>>> GetMyBookings([FromQuery] int? tourId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var bookings = await _tourBookingService.GetMyBookingsAsync(userId, tourId);
        return Ok(bookings);
    }

    // ── GET /api/tour-bookings/provider-bookings ──────────
    [HttpGet("provider-bookings")]
    [Authorize(Roles = "Provider")]
    public async Task<ActionResult<IEnumerable<TourBookingResponseDto>>> GetProviderBookings([FromQuery] string? status)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var bookings = await _tourBookingService.GetProviderBookingsAsync(userId, status);
        return Ok(bookings);
    }

    // ── PATCH /api/tour-bookings/{id}/confirm ─────────────
    [HttpPatch("{id}/confirm")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> ConfirmBooking(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        // Hứng kết quả tuple (bool Success, string Message) trả về từ Service
        var (success, message) = await _tourBookingService.ConfirmBookingAsync(userId, id);

        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // ── PATCH /api/tour-bookings/{id}/cancel ──────────────
    [HttpPatch("{id}/cancel")]
    [Authorize(Roles = "Tourist,Provider")] // Mở khóa cho cả 2 Role gọi vào
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        // Trích xuất chính xác vai trò dựa trên hàm IsInRole
        string userRole = "";
        if (User.IsInRole("Tourist")) userRole = "Tourist"; 
        else if (User.IsInRole("Provider")) userRole = "Provider";

        var (success, message) = await _tourBookingService.CancelBookingAsync(userId, id, userRole);

        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // ── PATCH /api/tour-bookings/{id}/complete ────────────
    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> CompleteBooking(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var (success, message) = await _tourBookingService.CompleteBookingAsync(userId, id);

        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }
}