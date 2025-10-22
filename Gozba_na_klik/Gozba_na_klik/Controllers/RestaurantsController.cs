using System.Linq;
using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Enums;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IUserService _userService;
        private readonly ILogger<RestaurantsController> _logger;

        public RestaurantsController(IRestaurantService restaurantService, IUserService userService, ILogger<RestaurantsController> logger)
        {
            _restaurantService = restaurantService;
            _userService = userService;
            _logger = logger;
        }

        // GET: api/restaurants
        // Prikaz svih restorana (admin upotreba)
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            IEnumerable<Restaurant> result = await _restaurantService.GetAllRestaurantsAsync();
            return Ok(result);
        }

        // GET: api/restaurants/{id}
        // Detalji jednog restorana po identifikatoru
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            Restaurant? restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return Ok(restaurant);
        }

        // POST: api/restaurants
        // Kreiranje novog restorana (admin)
        [HttpPost]
        public async Task<IActionResult> PostByAdminAsync([FromBody] RequestCreateRestaurantByAdminDto dto)
        {
            _logger.LogInformation("Creating new restaurant");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _restaurantService.CreateRestaurantByAdminAsync(dto);
            return Ok(result);
        }

        //PUT: api/restaurants/:{id}/admin-edit
        [HttpPut("{id}/admin-edit")]
        public async Task<IActionResult> UpdateByAdminAsync(int id, [FromBody] RequestUpdateRestaurantByAdminDto dto)
        {
            _logger.LogInformation("Updating existing restaurant");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _restaurantService.UpdateRestaurantByAdminAsync(id,dto);
            return Ok(result);
        }

        // GET: api/restaurants/my
        // Prikaz samo restorana koji pripadaju prijavljenom vlasniku (AC #1)
        [HttpGet("my")]
        public async Task<IActionResult> GetMyRestaurantsAsync(
            [FromHeader(Name = "X-User-Id")] int? userId)
        {
            // Provera hedera
            if (userId == null || userId <= 0)
            {
                return Unauthorized(new { message = "Nedostaje ili je neispravan X-User-Id header." });
            }

            User? user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!string.Equals(user.Role, "RestaurantOwner", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            IEnumerable<Restaurant> restaurants = await _restaurantService.GetRestaurantsByOwnerAsync(user.Id);


            IEnumerable<RestaurantListItemDto> list = restaurants.Select(r => new RestaurantListItemDto
            {
                Id = r.Id,
                Name = r.Name,
                PhotoUrl = r.PhotoUrl,
                OwnerId = r.OwnerId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });

            return Ok(list);
        }

        [HttpGet("filterSortPage")]
        public async Task<IActionResult> GetFilteredSortedPagedAsync(
        [FromQuery] RestaurantFilter filter,
        [FromQuery] int sortType = (int)RestaurantSortType.A_Z,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
            {
                var result = await _restaurantService.GetAllFilteredSortedPagedAsync(filter, sortType, page, pageSize);
                return Ok(result);
            }

        // PUT: api/restaurants/{id}
        // Izmena osnovnih podataka restorana uz proveru vlasništva (AC #2 i #3)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(
    int id,
    [FromForm] RestaurantUpdateDto dto,
    [FromHeader(Name = "X-User-Id")] int? userId)
        {
            // Provera hedera
            if (userId == null || userId <= 0)
            {
                return Unauthorized(new { message = "Nedostaje ili je neispravan X-User-Id header." });
            }

            User? user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return Unauthorized();
            }
            if (!string.Equals(user.Role, "RestaurantOwner", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            Restaurant? entity = await _restaurantService.GetRestaurantByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            // Provera vlasništva: vlasnik može menjati samo svoje restorane
            if (entity.OwnerId != user.Id)
            {
                return Forbid();
            }

            // Minimalna validacija imena
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 150)
            {
                return UnprocessableEntity(new { field = "name", error = "Naziv je obavezan i najviše 150 karaktera." });
            }

            // Ažuriranje polja
            entity.Name = dto.Name?.Trim();
            entity.Address = dto.Address?.Trim();
            entity.Description = dto.Description?.Trim();
            entity.Phone = dto.Phone?.Trim();

            // Ako je poslata nova fotografija
            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                string folderPath = Path.Combine("assets", "restaurantImg");
                Directory.CreateDirectory(folderPath);

                string fileName = $"{Guid.NewGuid()}_{dto.Photo.FileName}";
                string filePath = Path.Combine(folderPath, fileName);

                using FileStream stream = new FileStream(filePath, FileMode.Create);
                await dto.Photo.CopyToAsync(stream);

                entity.PhotoUrl = "/assets/restaurantImg/" + fileName;
            }

            entity.UpdatedAt = DateTime.UtcNow;

            Restaurant updated = await _restaurantService.UpdateRestaurantAsync(entity);

            // Vrati summary DTO
            return Ok(new RestaurantListItemDto
            {
                Id = updated.Id,
                Name = updated.Name,
                PhotoUrl = updated.PhotoUrl,
                OwnerId = updated.OwnerId,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            });
        }


        // DELETE: api/restaurants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            Restaurant? restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null) return NotFound();

            await _restaurantService.DeleteRestaurantAsync(id);
            return NoContent();
        }

        // POST: api/restaurants/{id}/workschedules
        [HttpPut("{id}/workschedules")]
        public async Task<IActionResult> UpdateWorkSchedulesAsync(int id, [FromBody] List<WorkScheduleDto> scheduleDtos)
        {
            try
            {
                List<WorkSchedule> schedules = scheduleDtos.Select(dto => new WorkSchedule
                {
                    DayOfWeek = (DayOfWeek)dto.DayOfWeek,
                    OpenTime = TimeSpan.Parse(dto.OpenTime),
                    CloseTime = TimeSpan.Parse(dto.CloseTime)
                }).ToList();

                await _restaurantService.UpdateWorkSchedulesAsync(id, schedules);
                return Ok(new { message = "Radno vreme uspešno ažurirano." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Greška pri obradi radnog vremena", error = ex.Message });
            }
        }

        // POST: api/restaurants/{id}/closeddates
        [HttpPost("{id}/closeddates")]
        public async Task<IActionResult> AddClosedDateAsync(int id, [FromBody] ClosedDate date)
        {
            date.RestaurantId = id;
            await _restaurantService.AddClosedDateAsync(id, date);
            return Ok(new { message = "Neradni datum uspešno dodat." });
        }

        // DELETE: api/restaurants/{id}/closeddates/{dateId}
        [HttpDelete("{id}/closeddates/{dateId}")]
        public async Task<IActionResult> RemoveClosedDateAsync(int id, int dateId)
        {
            await _restaurantService.RemoveClosedDateAsync(id, dateId);
            return NoContent();
        }
        // GET /api/publishers/sortTypes
        [HttpGet("sortTypes")]
        public async Task<IActionResult> GetSortTypes()
        {
            var sortTypes = await _restaurantService.GetSortTypesAsync();
            return Ok(sortTypes);
        }
    }
}