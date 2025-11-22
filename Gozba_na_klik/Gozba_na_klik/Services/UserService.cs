using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gozba_na_klik.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        private const string DefaultProfileImagePath = "/assets/profileImg/default_profile.png";

        public UserService(
            IUsersRepository userRepository,
            IFileService fileService,
            IMapper mapper,
            ILogger<UserService> logger,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _fileService = fileService;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.UserImage))
                {
                    user.UserImage = DefaultProfileImagePath;
                }
            }

            return users;
        }

        public async Task<ProfileDto> RegisterAsync(RegistrationDto data)
        {
            var user = _mapper.Map<User>(data);

            var result = await _userManager.CreateAsync(user, data.Password);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));

            // Assign default role User
            var roleRes = await _userManager.AddToRoleAsync(user, "User");
            if (!roleRes.Succeeded)
                throw new BadRequestException(string.Join("; ", roleRes.Errors.Select(e => e.Description)));

            // Generate token
            var rawToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(rawToken); // safer than WebUtility

            var apiUrl = _configuration["ApiUrl"];
            var confirmationLink = $"{apiUrl}/api/Users/confirm-email?userId={user.Id}&token={encodedToken}";

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Confirm your account",
                    $"Click here to confirm your account: <a href='{confirmationLink}'>Confirm Email</a>");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
            }

            return _mapper.Map<ProfileDto>(user);
        }

        public async Task<string> Login(LoginRequest data)
        {
            var user = await _userManager.FindByNameAsync(data.Username);
            if (user == null)
            {
                string msg = $"User with username '{data.Username}' not found.";
                throw new BadRequestException(msg);
            }

            var passwordMatch = await _userManager.CheckPasswordAsync(user, data.Password);
            if (!passwordMatch)
            {
                string msg = "Password is incorrect.";
                throw new BadRequestException(msg);
            }
            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!emailConfirmed)
            {
                string msg = "Email is not confirmed. Please confirm your email before logging in.";
                throw new BadRequestException(msg);
            }
            var token = await GenerateJwt(user);
            return token;
        }
        public async Task<ProfileDto> GetProfile(ClaimsPrincipal userPrincipal)
        {
            // Preuzimanje korisničkog imena iz tokena
            var username = userPrincipal.FindFirstValue("username");

            if (username == null)
            {
                string msg = "Username claim is missing in the token.";
                throw new BadRequestException(msg);
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                string msg = $"User with username '{username}' not found.";
                throw new NotFoundException(msg);
            }

            return _mapper.Map<ProfileDto>(user);
        }
        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new NotFoundException($"User with email '{email}' not found.");

            var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(rawToken);

            _logger.LogInformation("[RESET] Raw token generated: {RawToken}", rawToken);
            _logger.LogInformation("[RESET] Encoded token for email link: {EncodedToken}", encodedToken);

            var FrontendUrl = _configuration["FrontendUrl"];
            var link = $"{FrontendUrl}/reset-password?userId={user.Id}&token={encodedToken}";

            var body = $@"
<p>Hello {user.UserName},</p>
<p>You requested a password reset. Click the link below to set a new password:</p>
<p><a href='{link}'>Reset Password</a></p>
<p>If you didn’t request this, you can ignore this email.</p>";

            string subject = $"Reset password {user.UserName}";
            await _emailService.SendEmailAsync(email, subject, body);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto data)
        {
            var user = await _userManager.FindByIdAsync(data.UserId.ToString());
            if (user == null)
                throw new NotFoundException($"User with id '{data.UserId}' not found.");

            _logger.LogInformation("[RESET] Token received from client: {Token}", data.Token);

            // Fix + decode
            var decodedToken = Uri.UnescapeDataString(data.Token).Replace(" ", "+");

            _logger.LogInformation("[RESET] Token normalized: {DecodedToken}", decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, data.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("[RESET] Reset failed: {Errors}", errors);
                throw new BadRequestException(errors);
            }

            _logger.LogInformation("[RESET] Password reset succeeded");
            return result;
        }

        private async Task<string> GenerateJwt(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("username", user.UserName),
            new Claim("userid", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                // Roles
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<IEnumerable<User>> GetAllRestaurantOnwersAsync()
        {
            return await _userRepository.GetAllRestaurantOwnersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null && string.IsNullOrEmpty(user.UserImage))
            {
                user.UserImage = DefaultProfileImagePath;
            }

            return user;
        }

        public async Task<ResponseUserAlergenDto?> GetUserWithAlergensAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithAlergensAsync(userId);
            if (user == null) return null;

            return _mapper.Map<ResponseUserAlergenDto>(user);
        }

        
        public async Task<User?> UpdateUserByAdminAsync(int id, RequestUpdateUserByAdminDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return null;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole<int>(dto.Role));

            await _userManager.AddToRoleAsync(user, dto.Role);
            _logger.LogInformation("Updated user {UserId} to role {Role}", id, dto.Role);

            return user;
        }

        public async Task<User?> UpdateUserAsync(int id, UpdateUserDto dto, IFormFile? userimage)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            user.UserName = dto.Username;
            user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);
                if (!result.Succeeded)
                    throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            if (userimage != null && userimage.Length > 0)
                user.UserImage = await _fileService.SaveUserImageAsync(userimage);

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<ResponseUserAlergenDto?> UpdateUserAlergensAsync(int userId, RequestUpdateAlergenByUserDto dto)
        {
            await _userRepository.UpdateUserAlergensAsync(userId, dto.AlergensIds);

            var updatedUser = await _userRepository.GetByIdWithAlergensAsync(userId);
            if (updatedUser == null) return null;

            return _mapper.Map<ResponseUserAlergenDto>(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString()) != null;
        }

        public async Task<string?> GetUserRoleAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }

        public async Task<IEnumerable<User>> GetEmployeesByRestaurantAsync(int restaurantId)
        {
            var allUsers = await _userRepository.GetAllAsync();
            return allUsers.Where(u => u.RestaurantId == restaurantId);
        }

        public async Task SuspendEmployeeAsync(int employeeId)
        {
            var user = await _userRepository.GetByIdAsync(employeeId);
            if (user != null)
            {
                user.IsActive = false;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task ActivateEmployeeAsync(int employeeId)
        {
            var user = await _userRepository.GetByIdAsync(employeeId);
            if (user != null)
            {
                user.IsActive = true;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task<List<User>> GetAllAvailableCouriersAsync()
        {
            return await _userRepository.GetAllAvailableCouriersAsync();
        }

        public async Task AssignOrderToCourierAsync(int courierId, int orderId)
        {
            var existingCourier = await _userRepository.GetByIdAsync(courierId);
            if (existingCourier == null)
                throw new NotFoundException(courierId);

            _logger.LogInformation($"Kuriru sa ID-em {courierId} dodeljujem dostavu ID {orderId}.");
            existingCourier.ActiveOrderId = orderId;
            await _userRepository.UpdateAsync(existingCourier);
        }

        public async Task ReleaseOrderFromCourierAsync(int courierId)
        {
            var existingCourier = await _userRepository.GetByIdAsync(courierId);
            if (existingCourier == null)
                throw new NotFoundException(courierId);

            _logger.LogInformation($"Kuriru sa ID-em {courierId} skidam dostavu ID {existingCourier.ActiveOrderId}.");
            existingCourier.ActiveOrderId = null;
            await _userRepository.UpdateAsync(existingCourier);
        }
    }
}
