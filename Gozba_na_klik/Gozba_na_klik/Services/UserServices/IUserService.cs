using Gozba_na_klik.DTOs;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Services.UserServices
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetAllRestaurantOnwersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UserExistsAsync(int userId);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(int id, UpdateUserDto dto, IFormFile? userimage);
        Task DeleteUserAsync(int id);
    }
}
