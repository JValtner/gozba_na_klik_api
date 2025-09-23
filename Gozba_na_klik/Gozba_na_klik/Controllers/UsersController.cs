using System.Threading.Tasks;
using Gozba_na_klik.DTOs;
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
                var allUsers = await _usersRepository.GetAllAsync();

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
