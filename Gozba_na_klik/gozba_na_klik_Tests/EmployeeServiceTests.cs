using AutoMapper;
using Gozba_na_klik.DTOs;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace gozba_na_klik_Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IUsersRepository> _userRepoMock = new();
        private readonly Mock<IRestaurantRepository> _restaurantRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<EmployeeService>> _loggerMock = new();

        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _service = new EmployeeService(
                _userRepoMock.Object,
                _restaurantRepoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Theory]
        [InlineData(1, 7)]
        [InlineData(2, 8)]
        [InlineData(3, 9)]
        public async Task GetEmployeesByRestaurantAsync_ThrowsNotFoundException_WhenRestaurantNotFound(int restaurantId, int ownerId)
        {
            _restaurantRepoMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant)null);

            var ex = await Should.ThrowAsync<NotFoundException>(() =>
                _service.GetEmployeesByRestaurantAsync(restaurantId, ownerId));

            ex.Message.ShouldContain($"Restoran sa ID {restaurantId}");
        }

        [Theory]
        [InlineData(1, 7, 8)]
        [InlineData(2, 7, 9)]
        public async Task GetEmployeesByRestaurantAsync_ThrowsForbiddenException_WhenNotOwner(int restaurantId, int ownerId, int actualOwnerId)
        {
            var restaurant = new Restaurant { Id = restaurantId, Name = "Test", OwnerId = actualOwnerId };
            _restaurantRepoMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var ex = await Should.ThrowAsync<ForbiddenException>(() =>
                _service.GetEmployeesByRestaurantAsync(restaurantId, ownerId));

            ex.Message.ShouldBe("Nemate pristup zaposlenima ovog restorana.");
        }

        [Fact]
        public async Task GetEmployeesByRestaurantAsync_ReturnsEmployees_WhenValid()
        {
            var restaurantId = 1;
            var ownerId = 7;
            var restaurant = new Restaurant { Id = restaurantId, Name = "Test", OwnerId = ownerId };

            var users = new List<User>
            {
                new User { Id = 10, UserName = "emp1", Email = "e1@test.com", PasswordHash = "hash", RestaurantId = restaurantId },
                new User { Id = 11, UserName = "emp2", Email = "e2@test.com", PasswordHash = "hash", RestaurantId = restaurantId }
            };

            var dtos = new List<EmployeeListItemDto>
            {
                new EmployeeListItemDto { Id = 10, Username = "emp1", Email = "e1@test.com", Role = "RestaurantEmployee" },
                new EmployeeListItemDto { Id = 11, Username = "emp2", Email = "e2@test.com", Role = "DeliveryPerson" }
            };

            _restaurantRepoMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeListItemDto>>(It.IsAny<IEnumerable<User>>())).Returns(dtos);

            var result = await _service.GetEmployeesByRestaurantAsync(restaurantId, ownerId);

            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }

        [Theory]
        [InlineData(1, 7)]
        [InlineData(2, 8)]
        public async Task RegisterEmployeeAsync_ThrowsNotFoundException_WhenRestaurantNotFound(int restaurantId, int ownerId)
        {
            var dto = new RegisterEmployeeDto { Username = "test", Email = "test@test.com", Password = "pass", Role = "RestaurantEmployee" };
            _restaurantRepoMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant)null);

            var ex = await Should.ThrowAsync<NotFoundException>(() =>
                _service.RegisterEmployeeAsync(restaurantId, ownerId, dto));

            ex.Message.ShouldContain($"Restoran sa ID {restaurantId}");
        }

        [Theory]
        [InlineData(1, 7, 8)]
        [InlineData(2, 7, 9)]
        public async Task RegisterEmployeeAsync_ThrowsForbiddenException_WhenNotOwner(int restaurantId, int ownerId, int actualOwnerId)
        {
            var dto = new RegisterEmployeeDto { Username = "test", Email = "test@test.com", Password = "pass", Role = "RestaurantEmployee" };
            var restaurant = new Restaurant { Id = restaurantId, Name = "Test", OwnerId = actualOwnerId };

            _restaurantRepoMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

            var ex = await Should.ThrowAsync<ForbiddenException>(() =>
                _service.RegisterEmployeeAsync(restaurantId, ownerId, dto));

            ex.Message.ShouldBe("Nemate pristup ovom restoranu.");
        }

        [Fact]
        public async Task RegisterEmployeeAsync_ThrowsBadRequestException_WhenEmailExists()
        {
            var restaurantId = 1;
            var ownerId = 7;
            var dto = new RegisterEmployeeDto { Username = "test", Email = "existing@test.com", Password = "pass", Role = "RestaurantEmployee" };
            var restaurant = new Restaurant { Id = restaurantId, Name = "Test", OwnerId = ownerId };

            var existingUsers = new List<User>
            {
                new User { Id = 100, UserName = "existing", Email = "existing@test.com", PasswordHash = "hash" }
            };

            _restaurantRepoMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(existingUsers);

            var ex = await Should.ThrowAsync<BadRequestException>(() =>
                _service.RegisterEmployeeAsync(restaurantId, ownerId, dto));

            ex.Message.ShouldBe("Korisnik sa ovim email-om već postoji.");
        }

        [Fact]
        public async Task RegisterEmployeeAsync_CreatesEmployee_WhenValid()
        {
            var restaurantId = 1;
            var ownerId = 7;
            var dto = new RegisterEmployeeDto { Username = "newuser", Email = "new@test.com", Password = "pass", Role = "RestaurantEmployee" };
            var restaurant = new Restaurant { Id = restaurantId, Name = "Test", OwnerId = ownerId };

            var newUser = new User
            {
                Id = 100,
                UserName = dto.Username,
                Email = dto.Email,
                PasswordHash = "hash", // mocked, identity handles hashing in real scenario
                RestaurantId = restaurantId,
                IsActive = true
            };

            var resultDto = new EmployeeListItemDto
            {
                Id = 100,
                Username = dto.Username,
                Email = dto.Email,
                Role = dto.Role,
                IsActive = true
            };

            _restaurantRepoMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(newUser);
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(newUser);
            _mapperMock.Setup(m => m.Map<EmployeeListItemDto>(newUser)).Returns(resultDto);

            var result = await _service.RegisterEmployeeAsync(restaurantId, ownerId, dto);

            result.ShouldNotBeNull();
            result.Username.ShouldBe(dto.Username);
            result.Email.ShouldBe(dto.Email);
        }
    }
}
