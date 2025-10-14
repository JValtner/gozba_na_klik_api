using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Employee;
using Gozba_na_klik.DTOs.Employees;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories.RestaurantRepositories;
using Gozba_na_klik.Repositories.UserRepositories;

namespace Gozba_na_klik.Services.EmployeeServices
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IRestaurantRepository _restaurantRepository;

        public EmployeeService(IUsersRepository userRepository, IRestaurantRepository restaurantRepository)
        {
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<IEnumerable<User>> GetEmployeesByRestaurantAsync(int restaurantId, int ownerId)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new KeyNotFoundException("Restoran nije pronađen.");

            if (restaurant.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Nemate pristup zaposlenima ovog restorana.");

            var allUsers = await _userRepository.GetAllAsync();
            return allUsers.Where(u => u.RestaurantId == restaurantId);
        }

        public async Task<User> RegisterEmployeeAsync(int restaurantId, int ownerId, RegisterEmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Username, Email i Password su obavezni.");

            if (dto.Role != "RestaurantEmployee" && dto.Role != "DeliveryPerson")
                throw new ArgumentException("Role mora biti 'RestaurantEmployee' ili 'DeliveryPerson'.");

            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new KeyNotFoundException("Restoran nije pronađen.");

            if (restaurant.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Nemate pristup ovom restoranu.");

            var allUsers = await _userRepository.GetAllAsync();
            if (allUsers.Any(u => u.Email.ToLower() == dto.Email.ToLower()))
                throw new InvalidOperationException("Korisnik sa ovim email-om već postoji.");

            var newEmployee = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role,
                RestaurantId = restaurantId,
                IsActive = true
            };

            return await _userRepository.AddAsync(newEmployee);
        }

        public async Task<User> UpdateEmployeeAsync(int employeeId, int ownerId, UpdateEmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Username i Email su obavezni.");

            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Zaposleni nije pronađen.");

            if (employee.RestaurantId == null)
                throw new InvalidOperationException("Ovaj korisnik nije zaposleni.");

            var restaurant = await _restaurantRepository.GetByIdAsync(employee.RestaurantId.Value);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Nemate pristup ovom zaposlenom.");

            employee.Username = dto.Username;
            employee.Email = dto.Email;
            employee.Role = dto.Role;

            if (!string.IsNullOrEmpty(dto.Password))
                employee.Password = dto.Password;

            return await _userRepository.UpdateAsync(employee);
        }

        public async Task SuspendEmployeeAsync(int employeeId, int ownerId)
        {
            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Zaposleni nije pronađen.");

            if (employee.RestaurantId == null)
                throw new InvalidOperationException("Ovaj korisnik nije zaposleni.");

            var restaurant = await _restaurantRepository.GetByIdAsync(employee.RestaurantId.Value);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Nemate pristup ovom zaposlenom.");

            employee.IsActive = false;
            await _userRepository.UpdateAsync(employee);
        }

        public async Task ActivateEmployeeAsync(int employeeId, int ownerId)
        {
            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Zaposleni nije pronađen.");

            if (employee.RestaurantId == null)
                throw new InvalidOperationException("Ovaj korisnik nije zaposleni.");

            var restaurant = await _restaurantRepository.GetByIdAsync(employee.RestaurantId.Value);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Nemate pristup ovom zaposlenom.");

            employee.IsActive = true;
            await _userRepository.UpdateAsync(employee);
        }
    }
}