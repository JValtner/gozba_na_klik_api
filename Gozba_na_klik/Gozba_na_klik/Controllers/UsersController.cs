using System.Threading.Tasks;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Mvc;



namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        //private readonly UsersDbRepository _usersRepository;
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
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
            User new_user = await _userService.CreateUserAsync(user);
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

            if (!await _userService.UserExistsAsync(id))
            {
                return NotFound();
            }

            User updated_user = await _userService.UpdateUserAsync(user);
            return Ok(updated_user);
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
