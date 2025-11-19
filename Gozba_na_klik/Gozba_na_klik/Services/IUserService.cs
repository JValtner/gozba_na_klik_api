using System.Security.Claims;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Microsoft.AspNetCore.Identity;

namespace Gozba_na_klik.Services
{
    public interface IUserService
    {
        Task<string> Login(LoginRequest data);
        Task<ProfileDto> GetProfile(ClaimsPrincipal userPrincipal);
        Task <ProfileDto>RegisterAsync(RegistrationDto data);
        Task RequestPasswordResetAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto data);
        Task ActivateEmployeeAsync(int employeeId);
        Task AssignOrderToCourierAsync(int courierId, int orderId);
        Task<bool> DeleteUserAsync(int id);
        Task<List<User>> GetAllAvailableCouriersAsync();
        Task<IEnumerable<User>> GetAllRestaurantOnwersAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetEmployeesByRestaurantAsync(int restaurantId);
        Task<User?> GetUserByIdAsync(int id);
        Task<string?> GetUserRoleAsync(int userId);
        Task<ResponseUserAlergenDto?> GetUserWithAlergensAsync(int userId);
        Task ReleaseOrderFromCourierAsync(int courierId);
        Task SuspendEmployeeAsync(int employeeId);
        Task<ResponseUserAlergenDto?> UpdateUserAlergensAsync(int userId, RequestUpdateAlergenByUserDto dto);
        Task<User?> UpdateUserAsync(int id, UpdateUserDto dto, IFormFile? userimage);
        Task<User?> UpdateUserByAdminAsync(int id, RequestUpdateUserByAdminDto dto);
        Task<bool> UserExistsAsync(int userId);
    }
}