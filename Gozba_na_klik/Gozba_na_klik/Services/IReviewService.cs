using Gozba_na_klik.DTOs.Review;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;

namespace Gozba_na_klik.Services
{
    public interface IReviewService
    {
        Task<bool> CreateReviewAsync(CreateReviewDto dto, int userId);
        Task<PaginatedList<RestaurantReviewDto>> GetRestaurantReviewsAsync(int restaurantId, int page, int pageSize);
        Task<double> GetRestaurantAverageRatingAsync(int restaurantId);

        // CRUD additions
        Task<RestaurantReviewDto?> GetReviewByIdAsync(int id);
        Task<bool> UpdateReviewAsync(int id, CreateReviewDto dto, int userId);
        Task<bool> DeleteReviewAsync(int id);
        Task<List<int>> GetTop5BestRestaurantsAsync();
    }
}
