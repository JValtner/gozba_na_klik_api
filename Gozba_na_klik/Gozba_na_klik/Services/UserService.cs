using Gozba_na_klik.DTOs;
using Gozba_na_klik.Models;
namespace Gozba_na_klik.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IFileService _fileService;
        private IFileService? fileService;

        
        public UserService(IUsersRepository userRepository, IFileService fileService)
        {
            _userRepository = userRepository;
            _fileService = fileService;
        }
        // Fallback image path (relative)
        private const string DefaultProfileImagePath = "/assets/profileImg/default_profile.png";

        public UserService(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
            _fileService = fileService;
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

        // Vlasnici restorana
        public async Task<IEnumerable<User>> GetAllRestaurantOnwersAsync()
        {
            return await _userRepository.GetAllRestaurantOwnersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int authorId)
        {
            var user = await _userRepository.GetByIdAsync(authorId);

            if (user != null && string.IsNullOrEmpty(user.UserImage))
            {
                user.UserImage = DefaultProfileImagePath;
            }

            return user;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.AddAsync(user);
        }

        public async Task<User> UpdateUserAsync(int id, UpdateUserDto dto, IFormFile? userimage)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return null;

            // update fields
            user.Username = dto.Username;
            user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = dto.Password;

            // handle file upload
            if (userimage != null && userimage.Length > 0)
            {
                user.UserImage = await _fileService.SaveUserImageAsync(userimage);
            }

            return await _userRepository.UpdateAsync(user);

        }

        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteAsync(userId);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _userRepository.ExistsAsync(userId);
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
    }
}