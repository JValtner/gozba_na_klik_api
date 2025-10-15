using AutoMapper;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

public class AlergenServiceTests
{
    private readonly Mock<IAlergensRepository> _repoMock = new();
    private readonly Mock<IMealsRepository> _mealsMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<AlergenService>> _loggerMock = new();

    private readonly AlergenService _service;

    public AlergenServiceTests()
    {
        _service = new AlergenService(
            _repoMock.Object,
            _mealsMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Theory]
    [InlineData(1, 10, "Peanuts")]
    [InlineData(2, 20, "Milk")]
    [InlineData(3, 30, "Eggs")]
    public async Task AddAlergenToMealAsync_ReturnsMappedDto(int mealId, int alergenId, string name)
    {
        var domain = new Alergen { Id = alergenId, Name = name };
        var dto = new ResponseAlergenDto { Id = alergenId, Name = name };

        _repoMock.Setup(r => r.AddAlergenToMealAsync(mealId, alergenId)).ReturnsAsync(domain);
        _mapperMock.Setup(m => m.Map<ResponseAlergenDto>(domain)).Returns(dto);

        var result = await _service.AddAlergenToMealAsync(mealId, alergenId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(dto.Id);
        result.Name.ShouldBe(dto.Name);
    }

    [Theory]
    [InlineData(1, 99)]
    [InlineData(2, 999)]
    public async Task AddAlergenToMealAsync_ThrowsNotFound_WhenRepoReturnsNull(int mealId, int alergenId)
    {
        _repoMock.Setup(r => r.AddAlergenToMealAsync(mealId, alergenId)).ReturnsAsync((Alergen)null);

        var ex = await Should.ThrowAsync<NotFoundException>(() => _service.AddAlergenToMealAsync(mealId, alergenId));
        ex.Message.ShouldContain("Failed to add allergen");
    }

    [Theory]
    [InlineData(1, 10, "Peanuts")]
    [InlineData(2, 20, "Milk")]
    public async Task RemoveAlergenFromMealAsync_ReturnsMappedDto(int mealId, int alergenId, string name)
    {
        var domain = new Alergen { Id = alergenId, Name = name };
        var dto = new ResponseAlergenDto { Id = alergenId, Name = name };

        _repoMock.Setup(r => r.RemoveAlergenFromMealAsync(mealId, alergenId)).ReturnsAsync(domain);
        _mapperMock.Setup(m => m.Map<ResponseAlergenDto>(domain)).Returns(dto);

        var result = await _service.RemoveAlergenFromMealAsync(mealId, alergenId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(dto.Id);
        result.Name.ShouldBe(dto.Name);
    }

    [Theory]
    [InlineData(1, 99)]
    [InlineData(2, 999)]
    public async Task RemoveAlergenFromMealAsync_ThrowsNotFound_WhenRepoReturnsNull(int mealId, int alergenId)
    {
        _repoMock.Setup(r => r.RemoveAlergenFromMealAsync(mealId, alergenId)).ReturnsAsync((Alergen)null);

        var ex = await Should.ThrowAsync<NotFoundException>(() => _service.RemoveAlergenFromMealAsync(mealId, alergenId));
        ex.Message.ShouldContain("Failed to remove allergen");
    }

    [Fact]
    public async Task AddAlergenToMealAsync_ThrowsBadRequest_WhenAlreadyLinked()
    {
        _repoMock.Setup(r => r.AddAlergenToMealAsync(1, 1)).ThrowsAsync(new BadRequestException("Already linked"));

        var ex = await Should.ThrowAsync<BadRequestException>(() => _service.AddAlergenToMealAsync(1, 1));
        ex.Message.ShouldBe("Already linked");
    }

    [Fact]
    public async Task RemoveAlergenFromMealAsync_ThrowsBadRequest_WhenNotLinked()
    {
        _repoMock.Setup(r => r.RemoveAlergenFromMealAsync(1, 1)).ThrowsAsync(new BadRequestException("Not linked"));

        var ex = await Should.ThrowAsync<BadRequestException>(() => _service.RemoveAlergenFromMealAsync(1, 1));
        ex.Message.ShouldBe("Not linked");
    }
}