using Gozba_na_klik.DTOs.Review;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Gozba_na_klik.Utils;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewsRepository _repository;
        private readonly GozbaNaKlikDbContext _context;
        private readonly IFileService _fileService;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewsRepository repository,
            GozbaNaKlikDbContext context,
            IFileService fileService,
            ILogger<ReviewService> logger)
        {
            _repository = repository;
            _context = context;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<bool> CreateReviewAsync(CreateReviewDto dto, int userId)
        {
            var order = await _context.Orders
                .Include(o => o.Restaurant)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null)
                throw new NotFoundException($"Porudžbina sa ID-em {dto.OrderId} nije pronađena.");

            if (order.UserId != userId)
                throw new ForbiddenException("Možete oceniti samo svoje porudžbine.");

            if (order.Status != "ZAVRŠENO")
                throw new BadRequestException("Recenzija je dostupna samo za završene porudžbine.");

            if (await _repository.ReviewExistsForOrderAsync(dto.OrderId))
                throw new BadRequestException("Ova porudžbina već ima recenziju.");

            if (dto.RestaurantRating < 1 || dto.RestaurantRating > 5)
                throw new BadRequestException("Ocena restorana mora biti između 1 i 5.");

            if (dto.CourierRating < 1 || dto.CourierRating > 5)
                throw new BadRequestException("Ocena kurira mora biti između 1 i 5.");

            string? photoUrl = null;
            if (dto.RestaurantPhoto != null)
            {
                try
                {
                    photoUrl = await _fileService.SaveMealImageAsync(dto.RestaurantPhoto, "reviewImg");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška pri čuvanju fotografije recenzije");
                    throw new BadRequestException("Greška pri čuvanju fotografije: " + ex.Message);
                }
            }

            if (!order.DeliveryPersonId.HasValue)
                throw new BadRequestException("Porudžbina nema dodeljenog kurira.");

            var review = new Review
            {
                OrderId = dto.OrderId,
                RestaurantId = order.RestaurantId,
                CourierId = order.DeliveryPersonId.Value,
                RestaurantRating = dto.RestaurantRating,
                RestaurantComment = dto.RestaurantComment,
                RestaurantPhotoUrl = photoUrl,
                CourierRating = dto.CourierRating,
                CourierComment = dto.CourierComment,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddReviewAsync(review);
            _logger.LogInformation("Recenzija kreirana za porudžbinu {OrderId}", dto.OrderId);
            return true;
        }

        public async Task<PaginatedList<RestaurantReviewDto>> GetRestaurantReviewsAsync(int restaurantId, int page, int pageSize)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            var (reviews, totalCount) = await _repository.GetRestaurantReviewsAsync(restaurantId, page, pageSize);

            var reviewDtos = reviews.Select(r => new RestaurantReviewDto
            {
                Id = r.Id,
                RestaurantRating = r.RestaurantRating,
                RestaurantComment = r.RestaurantComment,
                RestaurantPhotoUrl = r.RestaurantPhotoUrl,
                CourierRating = r.CourierRating,
                CourierComment = r.CourierComment,
                CreatedAt = r.CreatedAt
            }).ToList();
            return new PaginatedList<RestaurantReviewDto>(reviewDtos, totalCount, page, pageSize);
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
                Id = review.Id,
                RestaurantRating = review.RestaurantRating,
                RestaurantComment = review.RestaurantComment,
                RestaurantPhotoUrl = review.RestaurantPhotoUrl,
                CourierRating = review.CourierRating,
                CourierComment = review.CourierComment,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<bool> UpdateReviewAsync(int id, CreateReviewDto dto, int userId)
        {
            var review = await _repository.GetReviewByIdAsync(id);
            if (review == null)
                throw new NotFoundException($"Recenzija sa ID-em {id} nije pronađena.");

            var order = await _context.Orders.FindAsync(review.OrderId);
            if (order == null || order.UserId != userId)
                throw new ForbiddenException("Možete ažurirati samo svoje recenzije.");

            if (dto.RestaurantRating < 1 || dto.RestaurantRating > 5)
                throw new BadRequestException("Ocena restorana mora biti između 1 i 5.");

            if (dto.CourierRating < 1 || dto.CourierRating > 5)
                throw new BadRequestException("Ocena kurira mora biti između 1 i 5.");

            if (dto.RestaurantPhoto != null)
            {
                if (!string.IsNullOrEmpty(review.RestaurantPhotoUrl))
                {
                    _fileService.DeleteFile(review.RestaurantPhotoUrl);
                }
                try
                {
                    review.RestaurantPhotoUrl = await _fileService.SaveMealImageAsync(dto.RestaurantPhoto, "reviewImg");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška pri čuvanju fotografije recenzije");
                    throw new BadRequestException("Greška pri čuvanju fotografije: " + ex.Message);
                }
            }

            review.RestaurantRating = dto.RestaurantRating;
            review.RestaurantComment = dto.RestaurantComment;
            review.CourierRating = dto.CourierRating;
            review.CourierComment = dto.CourierComment;

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

        public async Task<List<int>> GetTop5BestRestaurantsAsync()
        {
            return await _repository.GetTop5BestRatedRestaurantIdsAsync ();
        }
    }
}
