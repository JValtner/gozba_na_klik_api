using Gozba_na_klik.DTOs.DeliveryPersonSchedule;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [Route("api/delivery-persons/{deliveryPersonId}/schedule")]
    [ApiController]
    public class DeliveryPersonScheduleController : ControllerBase
    {
        private readonly IDeliveryPersonScheduleService _service;
        private readonly ILogger<DeliveryPersonScheduleController> _logger;

        public DeliveryPersonScheduleController(
            IDeliveryPersonScheduleService service,
            ILogger<DeliveryPersonScheduleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize(Policy = "DeliveryPerson")]
        [HttpGet]
        public async Task<ActionResult<WeeklyScheduleDto>> GetWeeklySchedule(int deliveryPersonId)
        {
            _logger.LogInformation("GET request for weekly schedule of delivery person {DeliveryPersonId}", deliveryPersonId);
            var result = await _service.GetWeeklyScheduleAsync(deliveryPersonId);
            return Ok(result);
        }

        [Authorize(Policy = "DeliveryPerson")]
        [HttpPost]
        public async Task<ActionResult<DeliveryScheduleDto>> CreateSchedule(
            int deliveryPersonId,
            [FromBody] CreateDeliveryScheduleDto dto)
        {
            _logger.LogInformation("POST request to create schedule for delivery person {DeliveryPersonId}", deliveryPersonId);
            var result = await _service.CreateScheduleAsync(deliveryPersonId, dto);
            return CreatedAtAction(nameof(GetWeeklySchedule), new { deliveryPersonId }, result);
        }

        [Authorize(Policy = "DeliveryPerson")]
        [HttpPut("{scheduleId}")]
        public async Task<ActionResult<DeliveryScheduleDto>> UpdateSchedule(
            int deliveryPersonId,
            int scheduleId,
            [FromBody] CreateDeliveryScheduleDto dto)
        {
            _logger.LogInformation("PUT request to update schedule {ScheduleId} for delivery person {DeliveryPersonId}",
                scheduleId, deliveryPersonId);
            var result = await _service.UpdateScheduleAsync(deliveryPersonId, scheduleId, dto);
            return Ok(result);
        }

        [Authorize(Policy = "DeliveryPerson")]
        [HttpDelete("{scheduleId}")]
        public async Task<ActionResult> DeleteSchedule(int deliveryPersonId, int scheduleId)
        {
            _logger.LogInformation("DELETE request for schedule {ScheduleId} of delivery person {DeliveryPersonId}",
                scheduleId, deliveryPersonId);
            await _service.DeleteScheduleAsync(deliveryPersonId, scheduleId);
            return NoContent();
        }
    }
}