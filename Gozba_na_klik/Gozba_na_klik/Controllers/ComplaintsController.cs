using Gozba_na_klik.DTOs.Complaints;
using Gozba_na_klik.Services;
using Gozba_na_klik.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintService _complaintService;
        private readonly ILogger<ComplaintsController> _logger;

        public ComplaintsController(
            IComplaintService complaintService,
            ILogger<ComplaintsController> logger)
        {
            _complaintService = complaintService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "RegisteredPolicy")]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintDto dto)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("POST request to create complaint for order {OrderId} by user {UserId}", dto.OrderId, userId);

            var complaint = await _complaintService.CreateComplaintAsync(dto, userId);
            return CreatedAtAction(nameof(CreateComplaint), new { id = complaint.Id }, complaint);
        }

        [HttpGet("order/{orderId}/exists")]
        [Authorize(Policy = "RegisteredPolicy")]
        public async Task<IActionResult> CheckComplaintExists([FromRoute] int orderId)
        {
            var userId = User.GetUserId();
            var exists = await _complaintService.HasComplaintForOrderAsync(orderId, userId);
            return Ok(new { exists });
        }

        [HttpGet("order/{orderId}")]
        [Authorize(Policy = "RegisteredPolicy")]
        public async Task<IActionResult> GetComplaintByOrderId([FromRoute] int orderId)
        {
            var userId = User.GetUserId();
            var complaint = await _complaintService.GetComplaintByOrderIdAsync(orderId, userId);
            
            if (complaint == null)
            {
                return NotFound("Žalba za ovu porudžbinu nije pronađena.");
            }

            return Ok(complaint);
        }

        [HttpGet("restaurant/my")]
        [Authorize(Policy = "RestaurantOwnerPolicy")]
        public async Task<IActionResult> GetMyRestaurantComplaints()
        {
            var userId = User.GetUserId();
            var complaints = await _complaintService.GetComplaintsByOwnerIdAsync(userId);
            return Ok(complaints);
        }
    }
}

