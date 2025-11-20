using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Gozba_na_klik.DTOs.Complaints;
using Gozba_na_klik.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gozba_na_klik.Repositories
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly IMongoCollection<ComplaintDocument> _complaints;
        private readonly ILogger<ComplaintRepository> _logger;

        public ComplaintRepository(IMongoDatabase database, ILogger<ComplaintRepository> logger)
        {
            _complaints = database.GetCollection<ComplaintDocument>("complaints");
            _logger = logger;

            CreateIndexes();
        }

        public async Task<ComplaintResponseDto> InsertComplaintAsync(CreateComplaintDto dto, int userId, int restaurantId)
        {
            try
            {
                var document = new ComplaintDocument
                {
                    Id = ObjectId.GenerateNewId(),
                    OrderId = dto.OrderId,
                    UserId = userId,
                    RestaurantId = restaurantId,
                    Message = dto.Message,
                    CreatedAt = DateTime.UtcNow
                };

                await _complaints.InsertOneAsync(document);
                _logger.LogInformation("Complaint saved to MongoDB for order {OrderId} by user {UserId}", dto.OrderId, userId);

                return new ComplaintResponseDto
                {
                    Id = document.Id.ToString(),
                    OrderId = document.OrderId,
                    UserId = document.UserId,
                    RestaurantId = document.RestaurantId,
                    Message = document.Message,
                    CreatedAt = document.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving complaint for order {OrderId} to MongoDB", dto.OrderId);
                throw;
            }
        }

        public async Task<bool> ComplaintExistsForOrderAsync(int orderId)
        {
            try
            {
                var filter = Builders<ComplaintDocument>.Filter.Eq(x => x.OrderId, orderId);
                var count = await _complaints.CountDocumentsAsync(filter);

                _logger.LogInformation("Complaint existence check for order {OrderId}: {Exists}", orderId, count > 0);
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking complaint existence for order ID {OrderId}", orderId);
                throw;
            }
        }

        public async Task<List<ComplaintResponseDto>> GetComplaintsByRestaurantIdAsync(int restaurantId)
        {
            try
            {
                var filter = Builders<ComplaintDocument>.Filter.Eq(x => x.RestaurantId, restaurantId);
                var complaints = await _complaints.Find(filter)
                    .SortByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var result = complaints.Select(c => new ComplaintResponseDto
                {
                    Id = c.Id.ToString(),
                    OrderId = c.OrderId,
                    UserId = c.UserId,
                    RestaurantId = c.RestaurantId,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} complaints for restaurant {RestaurantId}", result.Count, restaurantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving complaints for restaurant ID {RestaurantId}", restaurantId);
                throw;
            }
        }

        public async Task<bool> HasComplaintForOrderAsync(int orderId, int userId)
        {
            try
            {
                var filter = Builders<ComplaintDocument>.Filter.And(
                    Builders<ComplaintDocument>.Filter.Eq(x => x.OrderId, orderId),
                    Builders<ComplaintDocument>.Filter.Eq(x => x.UserId, userId)
                );
                var count = await _complaints.CountDocumentsAsync(filter);

                _logger.LogInformation("Complaint check for order {OrderId} by user {UserId}: {Exists}", orderId, userId, count > 0);
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking complaint for order {OrderId} by user {UserId}", orderId, userId);
                throw;
            }
        }

        public async Task<List<ComplaintResponseDto>> GetComplaintsByRestaurantIdsAsync(List<int> restaurantIds)
        {
            try
            {
                var filter = Builders<ComplaintDocument>.Filter.In(x => x.RestaurantId, restaurantIds);
                var complaints = await _complaints.Find(filter)
                    .SortByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var result = complaints.Select(c => new ComplaintResponseDto
                {
                    Id = c.Id.ToString(),
                    OrderId = c.OrderId,
                    UserId = c.UserId,
                    RestaurantId = c.RestaurantId,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} complaints for {RestaurantCount} restaurants", result.Count, restaurantIds.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving complaints for restaurant IDs");
                throw;
            }
        }

        public async Task<ComplaintResponseDto> GetComplaintByOrderIdAndUserIdAsync(int orderId, int userId)
        {
            try
            {
                var filter = Builders<ComplaintDocument>.Filter.And(
                    Builders<ComplaintDocument>.Filter.Eq(x => x.OrderId, orderId),
                    Builders<ComplaintDocument>.Filter.Eq(x => x.UserId, userId)
                );
                var complaint = await _complaints.Find(filter).FirstOrDefaultAsync();

                if (complaint == null)
                {
                    return null;
                }

                var result = new ComplaintResponseDto
                {
                    Id = complaint.Id.ToString(),
                    OrderId = complaint.OrderId,
                    UserId = complaint.UserId,
                    RestaurantId = complaint.RestaurantId,
                    Message = complaint.Message,
                    CreatedAt = complaint.CreatedAt
                };

                _logger.LogInformation("Retrieved complaint for order {OrderId} by user {UserId}", orderId, userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving complaint for order {OrderId} by user {UserId}", orderId, userId);
                throw;
            }
        }

        private void CreateIndexes()
        {
            try
            {
                var orderIdIndex = new CreateIndexModel<ComplaintDocument>(
                    Builders<ComplaintDocument>.IndexKeys.Ascending(x => x.OrderId));

                var userIdIndex = new CreateIndexModel<ComplaintDocument>(
                    Builders<ComplaintDocument>.IndexKeys.Ascending(x => x.UserId));

                var createdAtIndex = new CreateIndexModel<ComplaintDocument>(
                    Builders<ComplaintDocument>.IndexKeys.Descending(x => x.CreatedAt));

                _complaints.Indexes.CreateMany(new[] { orderIdIndex, userIdIndex, createdAtIndex });
                _logger.LogInformation("MongoDB indexes created for complaints collection");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error creating MongoDB indexes for complaints collection");
            }
        }
    }

    public class ComplaintDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("orderId")]
        public int OrderId { get; set; }

        [BsonElement("userId")]
        public int UserId { get; set; }

        [BsonElement("restaurantId")]
        public int RestaurantId { get; set; }

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}

