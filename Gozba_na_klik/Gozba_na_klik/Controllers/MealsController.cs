using Gozba_na_klik.Utils;
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
    public class MealsController : ControllerBase
    {
        private readonly IMealService _mealService;
        private readonly IFileService _fileService;

        public MealsController(IMealService mealService, IFileService fileService)
        {
            _mealService = mealService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            IEnumerable<ResponseMealDto> result = await _mealService.GetAllMealsAsync();
            return Ok(result);
        }
        // GET: api/meals/restaurant/3
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<ResponseMealDto>>> GetMealsByRestaurant(int restaurantId)
        {
            var meals = await _mealService.GetMealsByRestaurantIdAsync(restaurantId);
            return Ok(meals);
        }

        // GET: api/meals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseMealDto>> GetOneAsync(int id)
        {
            var meal = await _mealService.GetMealByIdAsync(id);
            if (meal == null)
                return NotFound();
            return Ok(meal);
        }
        // GET: api/meals/paged
        [HttpGet("filterSortPage")]
        public async Task<ActionResult<PaginatedList<ResponseMealDto>>> GetFilteredSortedPagedAsync(
        [FromQuery] MealFilter filter,
        [FromQuery] int sortType = (int)MealSortType.A_Z,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            var result = await _mealService.GetAllFilteredSortedPagedAsync(filter, sortType, page, pageSize);
            return Ok(result);
        }

        // POST: api/meals
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] RequestMealDto dto, IFormFile? mealImage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO to entity
            var meal = new Meal
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                RestaurantId = dto.RestaurantId
            };

            // Upload image if provided
            if (mealImage != null && mealImage.Length > 0)
                meal.ImagePath = await _fileService.SaveMealImageAsync(mealImage);

            var newMeal = await _mealService.CreateMealAsync(meal);
            return Ok(newMeal);
        }

        // PUT: api/meals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromForm] RequestMealDto dto, IFormFile? mealImage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var meal = new Meal
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                RestaurantId = dto.RestaurantId
            };

            if (mealImage != null && mealImage.Length > 0)
                meal.ImagePath = await _fileService.SaveMealImageAsync(mealImage);

            var updatedMeal = await _mealService.UpdateMealAsync(meal);
            return Ok(updatedMeal);
        }

        // DELETE: api/meals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _mealService.DeleteMealAsync(id);
            return NoContent();
        }
    }
}
