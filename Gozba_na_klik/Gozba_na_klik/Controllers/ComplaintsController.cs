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
            _logger.LogInformation("User {UserId} creating complaint for order {OrderId}", userId, dto.OrderId);
            var complaint = await _complaintService.CreateComplaintAsync(dto, userId);
            _logger.LogInformation("Complaint created successfully with ID {ComplaintId}", complaint.Id);
            return CreatedAtAction(nameof(CreateComplaint), new { id = complaint.Id }, complaint);
        }

        [HttpGet("order/{orderId}/exists")]
        [Authorize(Policy = "RegisteredPolicy")]
        public async Task<IActionResult> CheckComplaintExists([FromRoute] int orderId)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("User {UserId} checking if complaint exists for order {OrderId}", userId, orderId);
            var exists = await _complaintService.HasComplaintForOrderAsync(orderId, userId);
            return Ok(new { exists });
        }

        [HttpGet("order/{orderId}")]
        [Authorize(Policy = "RegisteredPolicy")]
        public async Task<IActionResult> GetComplaintByOrderId([FromRoute] int orderId)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("User {UserId} requesting complaint for order {OrderId}", userId, orderId);
            var complaint = await _complaintService.GetComplaintByOrderIdAsync(orderId, userId);
            return Ok(complaint);
        }

        [HttpGet("admin/all")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllComplaintsLast30Days(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Admin requesting complaints from last 30 days, page {Page}, pageSize {PageSize}", page, pageSize);
            var result = await _complaintService.GetAllComplaintsLast30DaysAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("admin/{complaintId}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetComplaintById([FromRoute] string complaintId)
        {
            _logger.LogInformation("Admin requesting complaint with ID {ComplaintId}", complaintId);
            var complaint = await _complaintService.GetComplaintByIdAsync(complaintId);
            return Ok(complaint);
        }
    }
}

