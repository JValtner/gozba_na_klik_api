using Gozba_na_klik.DTOs.Review;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Gozba_na_klik.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize(Policy = "UserPolicy")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _service;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService service, ILogger<ReviewsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateReview([FromForm] CreateReviewDto dto)
        //{
        //    var success = await _service.CreateReviewAsync(dto);
        //    if (!success)
        //        return BadRequest("Order not found, not completed, or already reviewed.");
        //    return Ok();
        //}
        // POST: api/reviews
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromForm] CreateReviewDto dto)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("User {UserId} creating review for order {OrderId}", userId, dto.OrderId);

            var success = await _service.CreateReviewAsync(dto, userId);
            if (!success)
                return BadRequest(new { message = "Greška pri kreiranju recenzije." });

            return Ok(new { message = "Recenzija uspešno kreirana." });
        }
    }
}
