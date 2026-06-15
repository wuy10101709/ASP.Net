using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Travel.Data;
using Travel.DTOs;
using Travel.Models;
using Travel.Modules.RoomBookings;

[Route("api/room-bookings")]
[ApiController]
[Authorize]
public class RoomBookingsController : ControllerBase
{
    private IRoomBookingService  _rbService;

    public RoomBookingsController (IRoomBookingService rbService){
        _rbService = rbService;
    }

    // ── POST /api/room-bookings ───────────────────────────
    [HttpPost]
    [Authorize(Roles = "Tourist")]
    public async Task<ActionResult<RoomBookingResponseDto>> CreateBooking(
        CreateRoomBookingDto request)
    {
         try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var roomBooking = await _rbService.BookingRoomAsync(userId, request);
            return Ok(roomBooking);
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

    // ── GET /api/room-bookings/my-bookings ────────────────
    [HttpGet("my-bookings")]
    [Authorize(Roles = "Tourist")]
    public async Task<ActionResult<IEnumerable<RoomBookingResponseDto>>> GetMyBookings()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var bookings = await _rbService.GetMyBookingAsync(userId);

        return Ok(bookings);
    }

    // ── GET /api/room-bookings/provider-bookings ──────────
    [HttpGet("provider-bookings")]
    [Authorize(Roles = "Provider")]
    public async Task<ActionResult<IEnumerable<RoomBookingResponseDto>>> GetProviderBookings(
        [FromQuery] string? status)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
       var bookings = await _rbService.GetProviderBookingAsync(userId, status);
        return Ok(bookings);
    }

    // ── PATCH /api/room-bookings/{id}/confirm ─────────────
    [HttpPatch("{id}/confirm")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> ConfirmBooking(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        // Hứng kết quả tuple (bool Success, string Message) trả về từ Service
        var (success, message) = await _rbService.ConfirmBookingAsync(userId, id);

        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // ── PATCH /api/room-bookings/{id}/cancel ──────────────
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        string userRole = "";
        if (User.IsInRole("Tourist")) userRole = "Tourist"; 
        else if (User.IsInRole("Provider")) userRole = "Provider";
        // Hứng kết quả tuple (bool Success, string Message) trả về từ Service
        var (success, message) = await _rbService.CancelBookingAsync(userId, id, userRole);

        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // ── PATCH /api/room-bookings/{id}/complete ────────────
    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> CompleteBooking(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
         var (success, message) = await _rbService.CompleteBookingAsync(userId, id);

        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }
}