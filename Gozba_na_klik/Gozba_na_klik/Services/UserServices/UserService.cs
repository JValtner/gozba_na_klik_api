using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories.UserRepositories;

namespace Gozba_na_klik.Services.UserServices
{
    public class UserService: IUserService
    {
        private readonly IUsersRepository _userRepository;

        public UserService(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int authorId)
        {
            return await _userRepository.GetByIdAsync(authorId);
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
