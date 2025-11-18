using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Employee;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "EmployeeOrAdminPolicy")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET: api/employee/restaurant/{restaurantId}
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<EmployeeListItemDto>>> GetEmployeesByRestaurant(
            int restaurantId,
            [FromHeader(Name = "X-User-Id")] int ownerId)
        {
            _logger.LogInformation("Fetching employees for restaurant {RestaurantId}", restaurantId);
            var employees = await _employeeService.GetEmployeesByRestaurantAsync(restaurantId, ownerId);
            return Ok(employees);
        }

        // POST: api/employee/restaurant/{restaurantId}&ownerId ={ownerId}
        [HttpPost("restaurant/{restaurantId}")]
        public async Task<ActionResult<EmployeeListItemDto>> RegisterEmployee(
            int restaurantId,[FromQuery] int ownerId,
            [FromBody] RegisterEmployeeDto dto)
        {
            _logger.LogInformation("Registering employee for restaurant {RestaurantId}", restaurantId);
            var employee = await _employeeService.RegisterEmployeeAsync(restaurantId, ownerId, dto);
            return Ok(employee);
        }

        // PUT: api/employee/{employeeId}
        [HttpPut("{employeeId}")]
        public async Task<ActionResult<EmployeeListItemDto>> UpdateEmployee(
            int employeeId,
            [FromHeader(Name = "X-User-Id")] int ownerId,
            [FromBody] UpdateEmployeeDto dto)
        {
            _logger.LogInformation("Updating employee {EmployeeId}", employeeId);
            var updated = await _employeeService.UpdateEmployeeAsync(employeeId, ownerId, dto);
            return Ok(updated);
        }

        // PUT: api/employee/{employeeId}/suspend
        [HttpPut("{employeeId}/suspend")]
        public async Task<IActionResult> SuspendEmployee(
            int employeeId,
            [FromHeader(Name = "X-User-Id")] int ownerId)
        {
            _logger.LogInformation("Suspending employee {EmployeeId}", employeeId);
            await _employeeService.SuspendEmployeeAsync(employeeId, ownerId);
            return NoContent();
        }

        // PUT: api/employee/{employeeId}/activate
        [HttpPut("{employeeId}/activate")]
        public async Task<IActionResult> ActivateEmployee(
            int employeeId,
            [FromHeader(Name = "X-User-Id")] int ownerId)
        {
            _logger.LogInformation("Activating employee {EmployeeId}", employeeId);
            await _employeeService.ActivateEmployeeAsync(employeeId, ownerId);
            return NoContent();
        }
    }
}