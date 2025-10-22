using Gozba_na_klik.Models;

public class ReviewService : IReviewService
{
    private readonly IReviewsRepository _repository;
    private readonly GozbaNaKlikDbContext _context;

    public ReviewService(IReviewsRepository repository, GozbaNaKlikDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<bool> CreateReviewAsync(CreateReviewDto dto)
    {
        var order = await _context.Orders.FindAsync(dto.OrderId);
        if (order == null || order.Status != "ZAVRŠENO")
            return false;

        if (await _repository.ReviewExistsForOrderAsync(dto.OrderId))
            return false;

        var review = new Review
        {
            OrderId = dto.OrderId,
            RestaurantId = order.RestaurantId,
            //CourierId = order.CourierId,
            RestaurantRating = dto.RestaurantRating,
            RestaurantComment = dto.RestaurantComment,
            RestaurantPhotoUrl = dto.RestaurantPhoto != null ? await SavePhotoAsync(dto.RestaurantPhoto) : null,
            CourierRating = dto.CourierRating,
            CourierComment = dto.CourierComment,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddReviewAsync(review);
        return true;
    }

    public async Task<List<RestaurantReviewDto>> GetRestaurantReviewsAsync(int restaurantId, int page, int pageSize)
    {
        var reviews = await _repository.GetRestaurantReviewsAsync(restaurantId, page, pageSize);
        return reviews.Select(r => new RestaurantReviewDto
        {
            RestaurantRating = r.RestaurantRating,
            RestaurantComment = r.RestaurantComment,
            RestaurantPhotoUrl = r.RestaurantPhotoUrl,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<double> GetRestaurantAverageRatingAsync(int restaurantId)
    {
        return await _repository.GetRestaurantAverageRatingAsync(restaurantId);
    }

    // CRUD additions
    public async Task<RestaurantReviewDto?> GetReviewByIdAsync(int id)
    {
        var review = await _repository.GetReviewByIdAsync(id);
        if (review == null) return null;
        return new RestaurantReviewDto
        {
            RestaurantRating = review.RestaurantRating,
            RestaurantComment = review.RestaurantComment,
            RestaurantPhotoUrl = review.RestaurantPhotoUrl,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task<bool> UpdateReviewAsync(int id, CreateReviewDto dto)
    {
        var review = await _repository.GetReviewByIdAsync(id);
        if (review == null) return false;

        review.RestaurantRating = dto.RestaurantRating;
        review.RestaurantComment = dto.RestaurantComment;
        review.RestaurantPhotoUrl = dto.RestaurantPhoto != null ? await SavePhotoAsync(dto.RestaurantPhoto) : review.RestaurantPhotoUrl;
        review.CourierRating = dto.CourierRating;
        review.CourierComment = dto.CourierComment;
        // Optionally update CreatedAt or other fields

        await _repository.UpdateReviewAsync(review);
        return true;
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        var review = await _repository.GetReviewByIdAsync(id);
        if (review == null) return false;
        await _repository.DeleteReviewAsync(id);
        return true;
    }

    // Example photo saving method
    private async Task<string?> SavePhotoAsync(IFormFile photo)
    {
        // Implement photo saving logic and return URL
        return null;
    }
}
