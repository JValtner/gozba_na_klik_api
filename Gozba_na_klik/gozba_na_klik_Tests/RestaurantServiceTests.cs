using AutoMapper;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace gozba_na_klik_Tests.Services
{
    public class RestaurantServiceTests
    {
        private readonly Mock<IRestaurantRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<RestaurantService>> _loggerMock = new();
        private readonly RestaurantService _service;

        public RestaurantServiceTests()
        {
            _service = new RestaurantService(
                _repoMock.Object,
                null!,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Theory]
        [InlineData(1, "Bella Italia", 7)]
        [InlineData(2, "Sushi Master", 8)]
        [InlineData(3, "Grill House", 9)]
        public async Task GetRestaurantByIdAsync_ReturnsRestaurant_WhenExists(int id, string name, int ownerId)
        {
            var restaurant = new Restaurant { Id = id, Name = name, OwnerId = ownerId };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(restaurant);

            var result = await _service.GetRestaurantByIdAsync(id);

            result.ShouldNotBeNull();
            result.Id.ShouldBe(id);
            result.Name.ShouldBe(name);
            result.OwnerId.ShouldBe(ownerId);
        }

        [Theory]
        [InlineData(999)]
        [InlineData(888)]
        public async Task GetRestaurantByIdAsync_ReturnsNull_WhenNotExists(int id)
        {
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Restaurant)null);

            var result = await _service.GetRestaurantByIdAsync(id);

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetRestaurantsByOwnerAsync_ReturnsRestaurants_WhenOwnerHasRestaurants()
        {
            var ownerId = 7;
            var restaurants = new List<Restaurant>
            {
                new Restaurant { Id = 1, Name = "Restaurant 1", OwnerId = ownerId },
                new Restaurant { Id = 2, Name = "Restaurant 2", OwnerId = ownerId }
            };

            _repoMock.Setup(r => r.GetByOwnerAsync(ownerId)).ReturnsAsync(restaurants);

            var result = await _service.GetRestaurantsByOwnerAsync(ownerId);

            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldAllBe(r => r.OwnerId == ownerId);
        }

        [Theory]
        [InlineData(999)]
        [InlineData(888)]
        public async Task GetRestaurantsByOwnerAsync_ReturnsEmpty_WhenOwnerHasNoRestaurants(int ownerId)
        {
            _repoMock.Setup(r => r.GetByOwnerAsync(ownerId)).ReturnsAsync(new List<Restaurant>());

            var result = await _service.GetRestaurantsByOwnerAsync(ownerId);

            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAllRestaurantsAsync_ReturnsAllRestaurants()
        {
            var restaurants = new List<Restaurant>
            {
                new Restaurant { Id = 1, Name = "Restaurant 1", OwnerId = 7 },
                new Restaurant { Id = 2, Name = "Restaurant 2", OwnerId = 8 },
                new Restaurant { Id = 3, Name = "Restaurant 3", OwnerId = 9 }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

            var result = await _service.GetAllRestaurantsAsync();

            result.ShouldNotBeNull();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public async Task CreateRestaurantAsync_CreatesAndReturnsRestaurant()
        {
            var newRestaurant = new Restaurant { Name = "New Restaurant", OwnerId = 7 };
            var createdRestaurant = new Restaurant { Id = 10, Name = newRestaurant.Name, OwnerId = newRestaurant.OwnerId };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Restaurant>())).ReturnsAsync(createdRestaurant);

            var result = await _service.CreateRestaurantAsync(newRestaurant);

            result.ShouldNotBeNull();
            result.Id.ShouldBe(10);
            result.Name.ShouldBe(newRestaurant.Name);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Restaurant>()), Times.Once);
        }

        [Fact]
        public async Task UpdateRestaurantAsync_UpdatesAndReturnsRestaurant()
        {
            var restaurant = new Restaurant { Id = 1, Name = "Updated Restaurant", OwnerId = 7 };
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Restaurant>())).ReturnsAsync(restaurant);

            var result = await _service.UpdateRestaurantAsync(restaurant);

            result.ShouldNotBeNull();
            result.Name.ShouldBe("Updated Restaurant");
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Restaurant>()), Times.Once);
        }

        [Fact]
        public async Task DeleteRestaurantAsync_CallsRepositoryDelete()
        {
            var restaurantId = 1;
            _repoMock.Setup(r => r.DeleteAsync(restaurantId)).ReturnsAsync(true);

            await _service.DeleteRestaurantAsync(restaurantId);

            _repoMock.Verify(r => r.DeleteAsync(restaurantId), Times.Once);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(999, false)]
        public async Task RestaurantExistsAsync_ReturnsCorrectValue(int id, bool expected)
        {
            _repoMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(expected);

            var result = await _service.RestaurantExistsAsync(id);

            result.ShouldBe(expected);
        }
    }
}