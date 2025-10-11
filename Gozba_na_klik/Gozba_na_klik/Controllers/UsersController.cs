using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Mvc;



namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public UsersController(IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
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
        public async Task<IActionResult> PutAsync(int id, [FromForm] UpdateUserDto dto, IFormFile? userimage)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, dto, userimage);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser);
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // Validacija input parametara
            if (string.IsNullOrWhiteSpace(loginRequest?.Username) || string.IsNullOrWhiteSpace(loginRequest?.Password))
            {
                return BadRequest(new { message = "Korisničko ime i lozinka su obavezni" });
            }

            try
            {
                // Dobij sve korisnike iz baze
                var allUsers = await _userService.GetAllUsersAsync();

                // Pronadji korisnika po username-u
                var user = allUsers.FirstOrDefault(u => u.Username.ToLower() == loginRequest.Username.ToLower());

                // Proveri da li korisnik postoji i da li se lozinka poklapa
                if (user == null || user.Password != loginRequest.Password)
                {
                    return Unauthorized(new { message = "Neispravno korisničko ime ili lozinka" });
                }

                // Kreiraj response objekat (bez lozinke)
                var loginResponse = new LoginResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška na serveru", details = ex.Message });
            }
        }
    }
}
