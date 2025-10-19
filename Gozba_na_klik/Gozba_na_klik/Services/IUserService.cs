using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllRestaurantOnwersAsync();
        Task<bool> UserExistsAsync(int userId);
        Task<User> CreateUserAsync(User user);

        // ADMIN 
        Task<User> UpdateUserByAdminAsync(int id, RequestUpdateUserByAdminDto dto);

        Task<User> UpdateUserAsync(int id, UpdateUserDto dto, IFormFile? userimage);
        Task DeleteUserAsync(int id);
    }
}
