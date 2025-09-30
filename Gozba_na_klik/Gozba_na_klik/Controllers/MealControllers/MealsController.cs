using Gozba_na_klik.DTOs;
using Gozba_na_klik.Models.MealModels;
using Gozba_na_klik.Services.FileServices;
using Gozba_na_klik.Services.MealServices;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers.MealControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        //private readonly MealsDbRepository _mealsRepository;
        private readonly IMealService _mealService;
        private readonly IFileService _fileService;

        public MealsController(IMealService mealService, IFileService fileService)
        {
            _mealService = mealService;
            _fileService = fileService;
        }
        // GET: api/meals
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _mealService.GetAllMealsAsync());
        }

        // GET api/meals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            Meal meal = await _mealService.GetMealByIdAsync(id);
            if (meal == null)
            {
                return NotFound();
            }
            return Ok(meal);
        }

        // POST api/meals
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] CreateMealDto dto, IFormFile? mealImage)
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

            // Optional image upload
            if (mealImage != null && mealImage.Length > 0)
            {
                meal.ImageUrl = await _fileService.SaveMealImageAsync(mealImage);
            }

            var newMeal = await _mealService.CreateMealAsync(meal);
            return Ok(newMeal);
        }

        // PUT api/meals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromForm] CreateMealDto dto, IFormFile? mealImage)
        {
            var meal = await _mealService.GetMealByIdAsync(id);
            if (meal == null) return NotFound();

            // Update fields from DTO
            meal.Name = dto.Name;
            meal.Description = dto.Description;
            meal.Price = dto.Price;
            meal.RestaurantId = dto.RestaurantId;

            // Optional image update
            if (mealImage != null && mealImage.Length > 0)
            {
                meal.ImageUrl = await _fileService.SaveMealImageAsync(mealImage);
            }

            var updatedMeal = await _mealService.UpdateMealAsync(meal);
            return Ok(updatedMeal);
        }



        // DELETE api/meals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var existingMeal = await _mealService.GetMealByIdAsync(id);
            if (existingMeal == null)
            {
                return NotFound();
            }
            await _mealService.DeleteMealAsync(id);
            return NoContent();
        }
    }
}