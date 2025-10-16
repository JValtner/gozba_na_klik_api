using AutoMapper;
using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Employee;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;

namespace Gozba_na_klik.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IUsersRepository userRepository,
            IRestaurantRepository restaurantRepository,
            IMapper mapper,
            ILogger<EmployeeService> logger)
        {
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeListItemDto>> GetEmployeesByRestaurantAsync(int restaurantId, int ownerId)
        {
            _logger.LogInformation("Fetching employees for restaurant {RestaurantId}", restaurantId);

            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new NotFoundException($"Restoran sa ID {restaurantId} nije pronađen.");

            if (restaurant.OwnerId != ownerId)
                throw new ForbiddenException("Nemate pristup zaposlenima ovog restorana.");

            var allUsers = await _userRepository.GetAllAsync();
            var employees = allUsers.Where(u => u.RestaurantId == restaurantId);

            return _mapper.Map<IEnumerable<EmployeeListItemDto>>(employees);
        }

        public async Task<EmployeeListItemDto> RegisterEmployeeAsync(int restaurantId, int ownerId, RegisterEmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                throw new BadRequestException("Username, Email i Password su obavezni.");

            if (dto.Role != "RestaurantEmployee" && dto.Role != "DeliveryPerson")
                throw new BadRequestException("Role mora biti 'RestaurantEmployee' ili 'DeliveryPerson'.");

            _logger.LogInformation("Registering employee for restaurant {RestaurantId}", restaurantId);

            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new NotFoundException($"Restoran sa ID {restaurantId} nije pronađen.");

            if (restaurant.OwnerId != ownerId)
                throw new ForbiddenException("Nemate pristup ovom restoranu.");

            var allUsers = await _userRepository.GetAllAsync();
            if (allUsers.Any(u => u.Email.ToLower() == dto.Email.ToLower()))
                throw new BadRequestException("Korisnik sa ovim email-om već postoji.");

            var newEmployee = _mapper.Map<User>(dto);
            newEmployee.RestaurantId = restaurantId;
            newEmployee.IsActive = true;

            var created = await _userRepository.AddAsync(newEmployee);
            return _mapper.Map<EmployeeListItemDto>(created);
        }

        public async Task<EmployeeListItemDto> UpdateEmployeeAsync(int employeeId, int ownerId, UpdateEmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email))
                throw new BadRequestException("Username i Email su obavezni.");

            _logger.LogInformation("Updating employee {EmployeeId}", employeeId);

            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new NotFoundException($"Zaposleni sa ID {employeeId} nije pronađen.");

            if (employee.RestaurantId == null)
                throw new BadRequestException("Ovaj korisnik nije zaposleni.");

            var restaurant = await _restaurantRepository.GetByIdAsync(employee.RestaurantId.Value);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new ForbiddenException("Nemate pristup ovom zaposlenom.");

            _mapper.Map(dto, employee);
            var updated = await _userRepository.UpdateAsync(employee);

            return _mapper.Map<EmployeeListItemDto>(updated);
        }

        public async Task SuspendEmployeeAsync(int employeeId, int ownerId)
        {
            _logger.LogInformation("Suspending employee {EmployeeId}", employeeId);

            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new NotFoundException($"Zaposleni sa ID {employeeId} nije pronađen.");

            if (employee.RestaurantId == null)
                throw new BadRequestException("Ovaj korisnik nije zaposleni.");

            var restaurant = await _restaurantRepository.GetByIdAsync(employee.RestaurantId.Value);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new ForbiddenException("Nemate pristup ovom zaposlenom.");

            employee.IsActive = false;
            await _userRepository.UpdateAsync(employee);
        }

        public async Task ActivateEmployeeAsync(int employeeId, int ownerId)
        {
            _logger.LogInformation("Activating employee {EmployeeId}", employeeId);

            var employee = await _userRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new NotFoundException($"Zaposleni sa ID {employeeId} nije pronađen.");

            if (employee.RestaurantId == null)
                throw new BadRequestException("Ovaj korisnik nije zaposleni.");

            var restaurant = await _restaurantRepository.GetByIdAsync(employee.RestaurantId.Value);
            if (restaurant == null || restaurant.OwnerId != ownerId)
                throw new ForbiddenException("Nemate pristup ovom zaposlenom.");

            employee.IsActive = true;
            await _userRepository.UpdateAsync(employee);
        }
    }
}