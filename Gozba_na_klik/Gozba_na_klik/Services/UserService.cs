using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
namespace Gozba_na_klik.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(IUsersRepository userRepository, IFileService fileService, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _fileService = fileService;
            _mapper = mapper;
            _logger = logger;
        }
        // Fallback image path (relative)
        private const string DefaultProfileImagePath = "/assets/profileImg/default_profile.png";

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

        // GET USER WITH ALERGENS
        public async Task<ResponseUserAlergenDto?> GetUserWithAlergensAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithAlergensAsync(userId);
            if (user == null)
                return null;

            return _mapper.Map<ResponseUserAlergenDto>(user);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.AddAsync(user);
        }

        // ADMIN UPDATE USER (ROLE CHANGE)
        public async Task<User> UpdateUserByAdminAsync(int id, RequestUpdateUserByAdminDto dto)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
                return null;

            user.Role = dto.Role;

            return await _userRepository.UpdateAsync(user);
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

        // UPDATE USER (ALERGENS)
        public async Task<ResponseUserAlergenDto?> UpdateUserAlergensAsync(int userId, RequestUpdateAlergenByUserDto dto)
        {
            await _userRepository.UpdateUserAlergensAsync(userId, dto.AlergensIds);

            // Ponovo učitaj entitet sa uključeniim alergenima
            var updatedUser = await _userRepository.GetByIdWithAlergensAsync(userId);
            if (updatedUser == null) return null;

            return _mapper.Map<ResponseUserAlergenDto>(updatedUser);
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

        // FIND ACTIVE FREE COURIER
        public async Task<List<User>> GetAllAvailableCouriersAsync()
        {
            return await _userRepository.GetAllAvailableCouriersAsync();
        }

        // ASSIGN ORDER TO COURIER
        public async Task AssignOrderToCourierAsync(int courierId, int orderId)
        {
            var existingCourier = await _userRepository.GetByIdAsync(courierId);
            if (existingCourier == null)
                throw new NotFoundException(courierId);

            _logger.LogInformation($"Kuriru sa ID-em {courierId} dodeljujem dostavu ID {orderId}.");
            existingCourier.ActiveOrderId = orderId;
            await _userRepository.UpdateAsync(existingCourier);
        }

        // REALEASE ORDER FROM COURIER
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