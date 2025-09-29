using Gozba_na_klik.DTOs;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.RestaurantModels;
using Gozba_na_klik.Models.Restaurants;
using Gozba_na_klik.Services.RestaurantServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers.RestaurantControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        // GET: api/restaurants
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _restaurantService.GetAllRestaurantsAsync());
        }

        // GET api/restaurants/3
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return Ok(restaurant);
        }

        // POST api/restaurants
        [HttpPost]
        public async Task<IActionResult> PostAsync(Restaurant restaurant)
        {
            Restaurant new_restaurant = await _restaurantService.CreateRestaurantAsync(restaurant);
            return Ok(new_restaurant);
        }

        // PUT: api/restaurants/3
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] string name, IFormFile? photo)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null) return NotFound();

            restaurant.Name = name;
            restaurant.UpdatedAt = DateTime.UtcNow;

            if (photo != null && photo.Length > 0)
            {
                var folderPath = Path.Combine("assets", "restaurants");
                Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}_{photo.FileName}";
                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await photo.CopyToAsync(stream);

                restaurant.PhotoUrl = "/" + filePath.Replace("\\", "/");
            }

            var updated = await _restaurantService.UpdateRestaurantAsync(restaurant);
            return Ok(updated);
        }

        // DELETE: api/restaurants/3
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null) return NotFound();

            await _restaurantService.DeleteRestaurantAsync(id);
            return NoContent();
        }

        // POST: api/restaurants/3/workschedules
        [HttpPost("{id}/workschedules")]
        public async Task<IActionResult> UpdateWorkSchedulesAsync(int id, [FromBody] List<WorkScheduleDto> scheduleDtos)
        {
            try
            {
                var schedules = scheduleDtos.Select(dto => new WorkSchedule
                {
                    DayOfWeek = (DayOfWeek)dto.DayOfWeek,
                    OpenTime = TimeSpan.Parse(dto.OpenTime),
                    CloseTime = TimeSpan.Parse(dto.CloseTime)
                }).ToList();

                await _restaurantService.UpdateWorkSchedulesAsync(id, schedules);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Greška pri obradi radnog vremena", error = ex.Message });
            }
        }

        // POST: api/restaurants/3/closeddates
        [HttpPost("{id}/closeddates")]
        public async Task<IActionResult> AddClosedDateAsync(int id, [FromBody] ClosedDate date)
        {
            await _restaurantService.AddClosedDateAsync(id, date);
            return Ok();
        }

        // DELETE: api/restaurants/3/closeddates/5
        [HttpDelete("{id}/closeddates/{dateId}")]
        public async Task<IActionResult> RemoveClosedDateAsync(int id, int dateId)
        {
            await _restaurantService.RemoveClosedDateAsync(id, dateId);
            return NoContent();
        }
    }
}
