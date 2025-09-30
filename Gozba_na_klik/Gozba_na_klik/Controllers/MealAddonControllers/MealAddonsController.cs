using Gozba_na_klik.Models.MealModels;
using Gozba_na_klik.Services.MealAddonServices;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers.MealAddonControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealAddonController : ControllerBase
    {
        //private readonly UsersDbRepository _usersRepository;
        private readonly IMealAddonService _mealAddonService;

        public MealAddonController(IMealAddonService mealAddonService)
        {
            _mealAddonService = mealAddonService;
        }
        // GET: api/meals
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _mealAddonService.GetAllMealAddonsAsync());
        }

        // GET api/meals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            MealAddon mealAddon = await _mealAddonService.GetMealAddonByIdAsync(id);
            if (mealAddon == null)
            {
                return NotFound();
            }
            return Ok(mealAddon);
        }

        // POST api/meals
        [HttpPost]
        public async Task<IActionResult> PostAsync(MealAddon mealAddon)
        {
            MealAddon new_mealAddon = await _mealAddonService.CreateMealAddonAsync(mealAddon);
            return Ok(new_mealAddon);
        }

        // PUT api/meals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] MealAddon mealAddon)
        {
            if (!await _mealAddonService.MealAddonExistsAsync(id)) 
                return NotFound();

            MealAddon updatedMealAddon = await _mealAddonService.UpdateMealAddonAsync(mealAddon);
            return Ok(mealAddon);
        }


        // DELETE api/meals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var existingMealAddon = await _mealAddonService.GetMealAddonByIdAsync(id);
            if (existingMealAddon == null)
            {
                return NotFound();
            }
            await _mealAddonService.DeleteMealAddonAsync(id);
            return NoContent();
        }
    }
}
