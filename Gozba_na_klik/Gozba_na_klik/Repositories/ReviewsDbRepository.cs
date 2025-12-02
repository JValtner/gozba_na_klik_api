using Gozba_na_klik.Models;
using Microsoft.EntityFrameworkCore;

public class ReviewsDbRepository : IReviewsRepository
{
    private readonly GozbaNaKlikDbContext _context;

    public ReviewsDbRepository(GozbaNaKlikDbContext context)
    {
        _context = context;
    }

    public async Task AddReviewAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ReviewExistsForOrderAsync(int orderId)
    {
        return await _context.Reviews.AnyAsync(r => r.OrderId == orderId);
    }

    public async Task<(List<Review> Reviews, int TotalCount)> GetRestaurantReviewsAsync(int restaurantId, int page, int pageSize)
    {
        var query = _context.Reviews
           .AsNoTracking()
           .Where(r => r.RestaurantId == restaurantId);

        var totalCount = await query.CountAsync();

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reviews, totalCount);
    }

    public async Task<double> GetRestaurantAverageRatingAsync(int restaurantId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.RestaurantId == restaurantId)
            .Select(r => (double)r.RestaurantRating)
            .ToListAsync();

        if (reviews.Count == 0)
            return 0.0;

        return reviews.Average();
    }

    // CRUD additions
    public async Task<Review?> GetReviewByIdAsync(int id)
    {
        return await _context.Reviews.FindAsync(id);
    }

    public async Task UpdateReviewAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<int>> GetTop5BestRatedRestaurantIdsAsync()
    {
        return await _context.Reviews
            .GroupBy(r => r.RestaurantId)
            .Select(g => new
            {
                RestaurantId = g.Key,
                AvgRating = g.Average(x => x.RestaurantRating),
                CountReviews = g.Count()
            })
            .OrderByDescending(x => x.AvgRating)
            .ThenByDescending(x => x.CountReviews)
            .Take(5)
            .Select(x => x.RestaurantId)
            .ToListAsync();
    }

}
