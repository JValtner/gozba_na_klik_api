using Microsoft.AspNetCore.Mvc;
using Gozba_na_klik.Services;
using Gozba_na_klik.DTOs.Response;

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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseAlergenDto>>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all allergens...");
            var alergens = await _alergenService.GetAllAlergenAsync();
            return Ok(alergens);
        }

        // GET: api/alergens/all (Koristi DTO koji sadrzi samo ID i naziv)
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ResponseAlergenBasicDto>>> GetAllAlergensAsync()
        {
            _logger.LogInformation("Fetching all allergens...");
            var alergens = await _alergenService.GetAllBasicAlergensAsync();
            return Ok(alergens);
        }

        // GET: api/alergens/meal/5
        [HttpGet("meal/{mealId:int}")]
        public async Task<ActionResult<IEnumerable<ResponseAlergenDto>>> GetByMealIdAsync(int mealId)
        {
            _logger.LogInformation("Fetching allergens for meal {MealId}", mealId);
            var alergens = await _alergenService.GetAlergenByMealIdAsync(mealId);
            return Ok(alergens);
        }

        // POST: api/alergens/{mealId}/{alergenId}
        [HttpPost("{mealId:int}/{alergenId:int}")]
        public async Task<ActionResult<ResponseAlergenDto>> AddToMealAsync(int mealId, int alergenId)
        {
            _logger.LogInformation("Adding allergen {AlergenId} to meal {MealId}", alergenId, mealId);
            var result = await _alergenService.AddAlergenToMealAsync(mealId, alergenId);
            return Ok(result);
        }

        // DELETE: api/alergens/{mealId}/{alergenId}
        [HttpDelete("{mealId:int}/{alergenId:int}")]
        public async Task<ActionResult<ResponseAlergenDto>> RemoveFromMealAsync(int mealId, int alergenId)
        {
            _logger.LogInformation("Removing allergen {AlergenId} from meal {MealId}", alergenId, mealId);
            var result = await _alergenService.RemoveAlergenFromMealAsync(mealId, alergenId);
            return Ok(result);
        }
    }
}
