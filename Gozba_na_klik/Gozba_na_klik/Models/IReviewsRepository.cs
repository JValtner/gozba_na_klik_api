using Gozba_na_klik.Models;

public interface IReviewsRepository
{
    Task AddReviewAsync(Review review);
    Task<bool> ReviewExistsForOrderAsync(int orderId);
    Task<List<Review>> GetRestaurantReviewsAsync(int restaurantId, int page, int pageSize);
    Task<double> GetRestaurantAverageRatingAsync(int restaurantId);

    // CRUD additions
    Task<Review?> GetReviewByIdAsync(int id);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(int id);
}
