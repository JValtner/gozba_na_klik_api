using Gozba_na_klik.DTOs;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories.UserRepositories;
using Gozba_na_klik.Services.FileServices;

namespace Gozba_na_klik.Services.UserServices
{
    public class UserService: IUserService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IFileService _fileService;

        public UserService(IUsersRepository userRepository, IFileService fileService)
        {
            _userRepository = userRepository;
            _fileService = fileService;
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
    }
}
