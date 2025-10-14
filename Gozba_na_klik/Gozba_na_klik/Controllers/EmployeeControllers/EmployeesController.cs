using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Employee;
using Gozba_na_klik.DTOs.Employees;
using Gozba_na_klik.Services.EmployeeServices; // ← promenio
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers.EmployeeControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService; // ← promenio

        public EmployeesController(IEmployeeService employeeService) // ← promenio
        {
            _employeeService = employeeService;
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetEmployeesByRestaurant(
            int restaurantId,
            [FromHeader(Name = "X-User-Id")] int? userId)
        {
            if (userId == null || userId <= 0)
                return Unauthorized(new { message = "Nedostaje ili je neispravan X-User-Id header." });

            try
            {
                var employees = await _employeeService.GetEmployeesByRestaurantAsync(restaurantId, userId.Value);

                var employeeDtos = employees.Select(e => new EmployeeListItemDto
                {
                    Id = e.Id,
                    Username = e.Username,
                    Email = e.Email,
                    Role = e.Role,
                    IsActive = e.IsActive,
                    UserImage = e.UserImage
                });

                return Ok(employeeDtos);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost("restaurant/{restaurantId}")]
        public async Task<IActionResult> RegisterEmployee(
            int restaurantId,
            [FromBody] RegisterEmployeeDto dto,
            [FromHeader(Name = "X-User-Id")] int? userId)
        {
            if (userId == null || userId <= 0)
                return Unauthorized(new { message = "Nedostaje ili je neispravan X-User-Id header." });

            try
            {
                var createdEmployee = await _employeeService.RegisterEmployeeAsync(restaurantId, userId.Value, dto);

                var resultDto = new EmployeeListItemDto
                {
                    Id = createdEmployee.Id,
                    Username = createdEmployee.Username,
                    Email = createdEmployee.Email,
                    Role = createdEmployee.Role,
                    IsActive = createdEmployee.IsActive,
                    UserImage = createdEmployee.UserImage
                };

                return Ok(resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            [FromBody] UpdateEmployeeDto dto,
            [FromHeader(Name = "X-User-Id")] int? userId)
        {
            if (userId == null || userId <= 0)
                return Unauthorized(new { message = "Nedostaje ili je neispravan X-User-Id header." });

            try
            {
                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, userId.Value, dto);

                var resultDto = new EmployeeListItemDto
                {
                    Id = updatedEmployee.Id,
                    Username = updatedEmployee.Username,
                    Email = updatedEmployee.Email,
                    Role = updatedEmployee.Role,
                    IsActive = updatedEmployee.IsActive,
                    UserImage = updatedEmployee.UserImage
                };

                return Ok(resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/suspend")]
        public async Task<IActionResult> SuspendEmployee(
            int id,
            [FromHeader(Name = "X-User-Id")] int? userId)
        {
            if (userId == null || userId <= 0)
                return Unauthorized(new { message = "Nedostaje ili je neispravan X-User-Id header." });

            try
            {
                await _employeeService.SuspendEmployeeAsync(id, userId.Value);
                return Ok(new { message = "Zaposleni je uspešno suspendovan." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateEmployee(
            int id,
            [FromHeader(Name = "X-User-Id")] int? userId)
        {
            if (userId == null || userId <= 0)
                return Unauthorized(new { message = "Nedostaje ili je neispravan X-User-Id header." });

            try
            {
                await _employeeService.ActivateEmployeeAsync(id, userId.Value);
                return Ok(new { message = "Zaposleni je uspešno aktiviran." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}