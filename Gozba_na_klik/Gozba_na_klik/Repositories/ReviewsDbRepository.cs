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

    public async Task<List<Review>> GetRestaurantReviewsAsync(int restaurantId, int page, int pageSize)
    {
        return await _context.Reviews
            .Where(r => r.RestaurantId == restaurantId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<double> GetRestaurantAverageRatingAsync(int restaurantId)
    {
        return await _context.Reviews
            .Where(r => r.RestaurantId == restaurantId)
            .AverageAsync(r => (double)r.RestaurantRating);
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
}
