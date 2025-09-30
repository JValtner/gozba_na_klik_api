using Gozba_na_klik.Models.MealModels;
using Gozba_na_klik.Services.AlergenServices;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers.AlergenControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlergensController : ControllerBase
    {
        //private readonly UsersDbRepository _usersRepository;
        private readonly IAlergenService _alergenService;

        public AlergensController(IAlergenService alergenService)
        {
            _alergenService = alergenService;
        }
        // GET: api/alergens
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _alergenService.GetAllAlergenAsync());
        }

        // GET api/alergens/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            var alergen = await _alergenService.GetAlergenByIdAsync(id);
            if (alergen == null)
            {
                return NotFound();
            }
            return Ok(alergen);
        }

        // POST api/alergens
        [HttpPost]
        public async Task<IActionResult> PostAsync(Alergen alergen)
        {
            Alergen new_alergen = await _alergenService.CreateAlergenAsync(alergen);
            return Ok(new_alergen);
        }

        // PUT api/alergens/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Alergen alergen)
        {
            if (!await _alergenService.AlergenExistsAsync(id))
                return NotFound();

            Alergen updatedAlergen = await _alergenService.UpdateAlergenAsync(alergen);
            return Ok(updatedAlergen);
        }


        // DELETE api/meals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var existingAlergen = await _alergenService.GetAlergenByIdAsync(id);
            if (existingAlergen == null)
            {
                return NotFound();
            }
            await _alergenService.DeleteAlergenAsync(id);
            return NoContent();
        }
    }
}
