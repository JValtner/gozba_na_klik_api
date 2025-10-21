using Gozba_na_klik.Models;
using Gozba_na_klik.Models;

public interface IReviewService
{
    Task<bool> CreateReviewAsync(CreateReviewDto dto);
    Task<List<RestaurantReviewDto>> GetRestaurantReviewsAsync(int restaurantId, int page, int pageSize);
    Task<double> GetRestaurantAverageRatingAsync(int restaurantId);

    // CRUD additions
    Task<RestaurantReviewDto?> GetReviewByIdAsync(int id);
    Task<bool> UpdateReviewAsync(int id, CreateReviewDto dto);
    Task<bool> DeleteReviewAsync(int id);
}

