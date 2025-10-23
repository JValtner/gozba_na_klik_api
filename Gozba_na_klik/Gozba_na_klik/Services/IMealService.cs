using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;

namespace Gozba_na_klik.Services
{
    public interface IMealService
    {
        Task<IEnumerable<ResponseMealDto>> GetAllMealsAsync();
        Task<IEnumerable<ResponseMealDto>> GetMealsByRestaurantIdAsync(int restaurantId);
        Task<ResponseMealDto?> GetMealByIdAsync(int mealId);
        Task<PaginatedList<ResponseMealDto>> GetAllFilteredSortedPagedAsync(MealFilter filter, int sortType, int page, int pageSize);
        Task<List<SortTypeOption>> GetSortTypesAsync();
        Task<bool> MealExistsAsync(int mealId);
        Task<ResponseMealDto> CreateMealAsync(Meal meal);
        Task<ResponseMealDto> UpdateMealAsync(Meal meal);
        Task DeleteMealAsync(int mealId);
        
    }
}
