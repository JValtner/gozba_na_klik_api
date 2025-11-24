using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Gozba_na_klik.DTOs.Complaints;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Gozba_na_klik.Repositories
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly IMongoCollection<ComplaintDocument> _complaints;
        private readonly ILogger<ComplaintRepository> _logger;
        private readonly IMongoDatabase _database;

        public ComplaintRepository(IMongoDatabase database, ILogger<ComplaintRepository> logger)
        {
            _database = database;
            _complaints = database.GetCollection<ComplaintDocument>("complaints");
            _logger = logger;
            _logger.LogInformation("ComplaintRepository initialized successfully");
        }
        
        private async Task<bool> IsMongoDbAvailableAsync()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
                await _database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cts.Token);
                return true;
            }
            catch
            {
                return false;
            }
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

        public async Task<bool> HasComplaintForOrderAsync(int orderId, int userId)
        {
            // Quick check if MongoDB is available - if not, return false immediately
            if (!await IsMongoDbAvailableAsync())
            {
                _logger.LogWarning("MongoDB not available - returning false for order {OrderId}, user {UserId}", orderId, userId);
                return false;
            }
            
            try
            {
                _logger.LogInformation("Checking complaint existence for order {OrderId} by user {UserId}", orderId, userId);
                
                var filter = Builders<ComplaintDocument>.Filter.And(
                    Builders<ComplaintDocument>.Filter.Eq(x => x.OrderId, orderId),
                    Builders<ComplaintDocument>.Filter.Eq(x => x.UserId, userId)
                );
                
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
                var result = await _complaints.Find(filter)
                    .Limit(1)
                    .FirstOrDefaultAsync(cts.Token);

                var exists = result != null;
                _logger.LogInformation("Complaint check for order {OrderId} by user {UserId}: {Exists}", orderId, userId, exists);
                return exists;
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("MongoDB timeout - returning false for order {OrderId}, user {UserId}", orderId, userId);
                return false;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("MongoDB timeout - returning false for order {OrderId}, user {UserId}", orderId, userId);
                return false;
            }
            catch (MongoException ex)
            {
                _logger.LogWarning(ex, "MongoDB unavailable - returning false for order {OrderId}, user {UserId}", orderId, userId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking complaint - returning false for order {OrderId}, user {UserId}", orderId, userId);
                return false;
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

        public async Task<PaginatedList<ComplaintResponseDto>> GetAllComplaintsLast30DaysAsync(int page, int pageSize)
        {
            try
            {
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var filter = Builders<ComplaintDocument>.Filter.Gte(x => x.CreatedAt, thirtyDaysAgo);
                
                // Get total count
                var totalCount = (int)await _complaints.CountDocumentsAsync(filter);
                
                // Get paginated results
                var complaints = await _complaints.Find(filter)
                    .SortByDescending(x => x.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Limit(pageSize)
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

                _logger.LogInformation("Retrieved {Count} complaints from last 30 days (page {Page}, pageSize {PageSize}, total {TotalCount})", 
                    result.Count, page, pageSize, totalCount);
                return new PaginatedList<ComplaintResponseDto>(result, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving complaints from last 30 days");
                throw;
            }
        }

        public async Task<ComplaintResponseDto> GetComplaintByIdAsync(string complaintId)
        {
            try
            {
                if (!ObjectId.TryParse(complaintId, out var objectId))
                {
                    _logger.LogWarning("Invalid complaint ID format: {ComplaintId}", complaintId);
                    return null;
                }

                var filter = Builders<ComplaintDocument>.Filter.Eq(x => x.Id, objectId);
                var complaint = await _complaints.Find(filter).FirstOrDefaultAsync();

                if (complaint == null)
                {
                    _logger.LogInformation("Complaint with ID {ComplaintId} not found", complaintId);
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

                _logger.LogInformation("Retrieved complaint with ID {ComplaintId}", complaintId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving complaint with ID {ComplaintId}", complaintId);
                throw;
            }
        }

        private async Task CreateIndexesAsync()
        {
            try
            {
                var orderIdIndex = new CreateIndexModel<ComplaintDocument>(
                    Builders<ComplaintDocument>.IndexKeys.Ascending(x => x.OrderId));

                var userIdIndex = new CreateIndexModel<ComplaintDocument>(
                    Builders<ComplaintDocument>.IndexKeys.Ascending(x => x.UserId));

                var createdAtIndex = new CreateIndexModel<ComplaintDocument>(
                    Builders<ComplaintDocument>.IndexKeys.Descending(x => x.CreatedAt));

                await _complaints.Indexes.CreateManyAsync(new[] { orderIdIndex, userIdIndex, createdAtIndex });
                _logger.LogInformation("MongoDB indexes created for complaints collection");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error creating MongoDB indexes for complaints collection");
            }
        }
    }

}

