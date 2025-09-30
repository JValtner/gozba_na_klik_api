using Gozba_na_klik.DTOs;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Restaurants;
using Gozba_na_klik.Services.UserServices;
using Gozba_na_klik.Services.RestaurantServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Gozba_na_klik.Controllers.RestaurantControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IUserService _userService;

        public RestaurantsController(IRestaurantService restaurantService, IUserService userService)
        {
            _restaurantService = restaurantService;
            _userService = userService;
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
        // Kreiranje novog restorana (tipično admin)
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Restaurant restaurant)
        {
            // Opcionalno: osigurati CreatedAt
            restaurant.CreatedAt = DateTime.UtcNow;
            Restaurant created = await _restaurantService.CreateRestaurantAsync(restaurant);
            return Ok(created);
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

            // Mapiranje na DTO da bismo izbegli anonimne tipove (i time upotrebu 'var')
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


        // PUT: api/restaurants/{id}
        // Izmena osnovnih podataka restorana uz proveru vlasništva (AC #2)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(
            int id,
            [FromBody] RestaurantUpdateDto dto,
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
            entity.Name = dto.Name.Trim();
            if (!string.IsNullOrWhiteSpace(dto.PhotoUrl))
            {
                entity.PhotoUrl = dto.PhotoUrl.Trim();
            }
            entity.UpdatedAt = DateTime.UtcNow;

            Restaurant updated = await _restaurantService.UpdateRestaurantAsync(entity);

            // Vraćamo sažetak (može i ceo entitet po potrebi)
            RestaurantListItemDto summary = new RestaurantListItemDto
            {
                Id = updated.Id,
                Name = updated.Name,
                PhotoUrl = updated.PhotoUrl,
                OwnerId = updated.OwnerId,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            };

            return Ok(summary);
        }

    }
}
