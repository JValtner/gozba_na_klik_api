using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AlergensController : ControllerBase
    {
        private readonly IAlergenService _alergenService;
        private readonly ILogger<AlergensController> _logger;

        public AlergensController(IAlergenService alergenService, ILogger<AlergensController> logger)
        {
            _alergenService = alergenService;
            _logger = logger;
        }

        // GET: api/alergens
        [Authorize(Policy = "PublicPolicy")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseAlergenDto>>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all allergens...");
            var alergens = await _alergenService.GetAllAlergenAsync();
            return Ok(alergens);
        }

        // GET: api/alergens/all (Koristi DTO koji sadrzi samo ID i naziv)
        [Authorize(Policy = "PublicPolicy")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ResponseAlergenBasicDto>>> GetAllAlergensAsync()
        {
            _logger.LogInformation("Fetching all allergens...");
            var alergens = await _alergenService.GetAllBasicAlergensAsync();
            return Ok(alergens);
        }


        // GET: api/alergens/meal/5
        [Authorize(Policy = "PublicPolicy")]
        [HttpGet("meal/{mealId:int}")]
        public async Task<ActionResult<IEnumerable<ResponseAlergenDto>>> GetByMealIdAsync(int mealId)
        {
            _logger.LogInformation("Fetching allergens for meal {MealId}", mealId);
            var alergens = await _alergenService.GetAlergenByMealIdAsync(mealId);
            return Ok(alergens);
        }

        // POST: api/alergens/{mealId}/{alergenId}
        [Authorize(Policy = "RestaurantOwnerPolicy")]
        [HttpPost("{mealId:int}/{alergenId:int}")]
        public async Task<ActionResult<ResponseAlergenDto>> AddToMealAsync(int mealId, int alergenId)
        {
            _logger.LogInformation("Adding allergen {AlergenId} to meal {MealId}", alergenId, mealId);
            var result = await _alergenService.AddAlergenToMealAsync(mealId, alergenId);
            return Ok(result);
        }

        // DELETE: api/alergens/{mealId}/{alergenId}
        [Authorize(Policy = "RestaurantOwnerPolicy")]
        [HttpDelete("{mealId:int}/{alergenId:int}")]
        public async Task<ActionResult<ResponseAlergenDto>> RemoveFromMealAsync(int mealId, int alergenId)
        {
            _logger.LogInformation("Removing allergen {AlergenId} from meal {MealId}", alergenId, mealId);
            var result = await _alergenService.RemoveAlergenFromMealAsync(mealId, alergenId);
            return Ok(result);
        }
    }
}
