using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Services; // Add this if IUserService is in the same namespace
namespace Gozba_na_klik.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;

        // Fallback image path (relative)
        private const string DefaultProfileImagePath = "/assets/profileImg/default_profile.png";

        public UserService(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
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

        public async Task<User> UpdateUserAsync(User author)
        {
            return await _userRepository.UpdateAsync(author);
        }

        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteAsync(userId);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _userRepository.ExistsAsync(userId);
        }
    }
}