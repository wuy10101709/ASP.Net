using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel.DTOs;
using Travel.Modules.Reviews;

namespace Travel.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // POST /api/reviews/tour
    [HttpPost("tour")]
    [Authorize(Roles = "Tourist")]
    public async Task<IActionResult> ReviewTour(CreateTourReviewDto request)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _reviewService
            .CreateTourReviewAsync(userId, request);

        return Ok(result);
    }

    // POST /api/reviews/accommodation
    [HttpPost("accommodation")]
    [Authorize(Roles = "Tourist")]
    public async Task<IActionResult> ReviewAccommodation(
        CreateAccommodationReviewDto request)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _reviewService
            .CreateAccommodationReviewAsync(userId, request);

        return Ok(result);
    }

    // GET /api/reviews/tour/{tourId}
    [HttpGet("tour/{tourId}")]
    public async Task<IActionResult> GetTourReviews(int tourId)
    {
        var result = await _reviewService.GetTourReviewsAsync(tourId);
        return Ok(result);
    }

    // GET /api/reviews/accommodation/{accommodationId}
    [HttpGet("accommodation/{accommodationId}")]
    public async Task<IActionResult> GetAccommodationReviews(int accommodationId)
    {
        var result = await _reviewService
            .GetAccommodationReviewsAsync(accommodationId);
        return Ok(result);
    }

    // DELETE /api/reviews/{id} — Admin xóa review vi phạm
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var (success, message) = await _reviewService.DeleteReviewAsync(id);

        if (!success) return NotFound(message);

        return Ok(new { message });
    }
}