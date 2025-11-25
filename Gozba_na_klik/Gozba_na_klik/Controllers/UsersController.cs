using System.Net;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Hubs;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Gozba_na_klik.Services.EmailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;



namespace Gozba_na_klik.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/users")]

    [ApiController]
    
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IHubContext<CourierLocationHub> _hub;

        public UsersController(
            IUserService userService,
            IFileService fileService,
            UserManager<User> userManager,
            IEmailService emailService,
            IHubContext<CourierLocationHub> hub)
        {
            _userService = userService;
            _fileService = fileService;
            _userManager = userManager;
            _emailService = emailService;
            _hub = hub;
        }

        // GET: api/users
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        // GET api/users/restaurant-owners
        [Authorize(Policy = "OwnerOrAdminPolicy")]
        [HttpGet("restaurant-owners")]
        public async Task<IActionResult> GetAllOwnersAsync()
        {
            return Ok(await _userService.GetAllRestaurantOnwersAsync());
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

        // GET api/users/5/alergens   (User sa svojim alergenima)
        [HttpGet("{id}/alergens")]
        public async Task<IActionResult> GetUserWithAlergens(int id)
        {
            return Ok(await _userService.GetUserWithAlergensAsync(id));

        }

        // POST api/users
        [Authorize(Policy = "PublicPolicy")]
        [HttpPost]
        public async Task<IActionResult> PostAsync(RegistrationDto registrationData)
        {
            ProfileDto profile = await _userService.RegisterAsync(registrationData);
            //Za sada vraca samo ok s obzirom da treba da se uradi validacija preko email!!!
            return Ok("User registered successfully");
        }

        // PUT api/users/5/alergens   (User dodaje sebi alergene)
        [Authorize(Policy = "PublicPolicy")]
        [HttpPut("{id}/alergens")]
        public async Task<IActionResult> PutUserAlergens(int id, [FromBody] RequestUpdateAlergenByUserDto dto)
        {
            var updatedUser = await _userService.UpdateUserAlergensAsync(id, dto);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        // ADMIN PUT api/users/5/admin-users
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}/admin-users")]
        public async Task<IActionResult> PutByAdminAsync(int id, [FromBody] RequestUpdateUserByAdminDto dto)
        {
            var updatedUser = await _userService.UpdateUserByAdminAsync(id, dto);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser);
        }

        // PUT api/users/5
        [Authorize(Policy = "RegisteredPolicy")]
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
        [Authorize(Policy = "AdminPolicy")]
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

        [Authorize(Policy = "PublicPolicy")]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _userService.Login(data);
            return Ok(token);
        }

        [Authorize(Policy = "RegisteredPolicy")]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            return Ok(await _userService.GetProfile(User));
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] int userId, [FromQuery] string token)
        {
            await _emailService.ConfirmEmailAsync(userId, token);
            return Ok("Email confirmed successfully");
        }

        [AllowAnonymous]
        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto dto)
        {
            await _userService.RequestPasswordResetAsync(dto.Email);
            return Ok("Password reset link sent.");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _userService.ResetPasswordAsync(dto);
            return Ok("Password successfully changed");
        }

        // Temporary diagnostic controller action
        [AllowAnonymous]
        [HttpPost("diagnose-reset-token")]
        public async Task<IActionResult> DiagnoseResetToken([FromBody] int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User not found");

            // Generate token and immediately verify — no email, no transport.
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var verify = await _userManager.ResetPasswordAsync(user, token, "Temp_Test@12345");

            return Ok(new
            {
                userId,
                success = verify.Succeeded,
                errors = verify.Succeeded ? null : verify.Errors.Select(e => e.Description).ToArray(),
                tokenSample = token.Substring(0, Math.Min(token.Length, 32)) // just to see shape
            });
        }

        [Authorize(Policy = "DeliveryPerson")]
        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateCourierLocation(int courierId, double latitude, double longitude)
        {
            var courier = await _userService.UpdateCourierLocation(courierId, latitude, longitude);
            await _hub.Clients.Group($"order-{courier.ActiveOrderId}")
                .SendAsync("ReceiveLocation", courier);

            return Ok(new { Message = "Location updated", courier });
        }

    }
}
