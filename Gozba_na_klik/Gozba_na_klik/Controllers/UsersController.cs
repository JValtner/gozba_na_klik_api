using System.Threading.Tasks;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repository;
using Microsoft.AspNetCore.Mvc;



namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersDbRepository _usersRepository;

        public UsersController(UsersDbRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _usersRepository.GetAllAsync());
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            User user = await _usersRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> PostAsync(User user)
        {
            User new_user = await _usersRepository.AddAsync(user);
            return Ok(new_user);
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            if (!await _usersRepository.ExistsAsync(id))
            {
                return NotFound();
            }

            User updated_user = await _usersRepository.UpdateAsync(user);
            return Ok(updated_user);
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            User existingUser = await _usersRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            await _usersRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
