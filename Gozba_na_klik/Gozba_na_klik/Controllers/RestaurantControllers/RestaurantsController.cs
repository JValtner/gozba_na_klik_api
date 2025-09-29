using Gozba_na_klik.DTOs;
using Gozba_na_klik.Models;
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

    }
}
