using Gozba_na_klik.DTOs.Review;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models;
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

        public ReviewsController(IReviewService service)
        {
            _service = service;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateReview([FromForm] CreateReviewDto dto)
        //{
        //    var success = await _service.CreateReviewAsync(dto);
        //    if (!success)
        //        return BadRequest("Order not found, not completed, or already reviewed.");
        //    return Ok();
        //}
    }
}
