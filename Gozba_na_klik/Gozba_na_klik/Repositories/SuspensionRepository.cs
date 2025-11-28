using MongoDB.Driver;
using MongoDB.Bson;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;

namespace Gozba_na_klik.Repositories
{
    public class SuspensionRepository : ISuspensionRepository
    {
        private readonly IMongoCollection<SuspensionDocument> _suspensions;
        private readonly ILogger<SuspensionRepository> _logger;
        private readonly IMongoDatabase _database;

        public SuspensionRepository(IMongoDatabase database, ILogger<SuspensionRepository> logger)
        {
            _database = database;
            _suspensions = database.GetCollection<SuspensionDocument>("suspensions");
            _logger = logger;
            _logger.LogInformation("SuspensionRepository initialized successfully");
        }

        public async Task<SuspensionResponseDto> InsertSuspensionAsync(int restaurantId, string reason, int adminId)
        {
            try
            {
                var document = new SuspensionDocument
                {
                    Id = ObjectId.GenerateNewId(),
                    RestaurantId = restaurantId,
                    SuspensionReason = reason,
                    SuspendedAt = DateTime.UtcNow,
                    SuspendedBy = adminId,
                    Status = "SUSPENDED"
                };

                await _suspensions.InsertOneAsync(document);
                _logger.LogInformation("Suspension saved to MongoDB for restaurant {RestaurantId} by admin {AdminId}", restaurantId, adminId);

                return new SuspensionResponseDto
                {
                    Id = document.Id.ToString(),
                    RestaurantId = document.RestaurantId,
                    SuspensionReason = document.SuspensionReason,
                    SuspendedAt = document.SuspendedAt,
                    SuspendedBy = document.SuspendedBy,
                    Status = document.Status
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving suspension for restaurant {RestaurantId} to MongoDB", restaurantId);
                throw;
            }
        }

        public async Task<SuspensionResponseDto?> GetSuspensionByRestaurantIdAsync(int restaurantId)
        {
            try
            {
                var activeStatuses = new[] { "SUSPENDED", "APPEALED", "REJECTED" };
                var filter = Builders<SuspensionDocument>.Filter.And(
                    Builders<SuspensionDocument>.Filter.Eq(x => x.RestaurantId, restaurantId),
                    Builders<SuspensionDocument>.Filter.In(x => x.Status, activeStatuses)
                );
                
                var suspension = await _suspensions.Find(filter)
                    .SortByDescending(x => x.SuspendedAt)
                    .FirstOrDefaultAsync();

                if (suspension == null)
                {
                    return null;
                }

                return new SuspensionResponseDto
                {
                    Id = suspension.Id.ToString(),
                    RestaurantId = suspension.RestaurantId,
                    SuspensionReason = suspension.SuspensionReason,
                    SuspendedAt = suspension.SuspendedAt,
                    SuspendedBy = suspension.SuspendedBy,
                    Status = suspension.Status,
                    AppealText = suspension.AppealText,
                    AppealDate = suspension.AppealDate,
                    DecisionDate = suspension.DecisionDate,
                    DecisionBy = suspension.DecisionBy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving suspension for restaurant {RestaurantId}", restaurantId);
                throw;
            }
        }

        public async Task<SuspensionResponseDto?> UpdateSuspensionWithAppealAsync(int restaurantId, string appealText, int ownerId)
        {
            try
            {
                var filter = Builders<SuspensionDocument>.Filter.And(
                    Builders<SuspensionDocument>.Filter.Eq(x => x.RestaurantId, restaurantId),
                    Builders<SuspensionDocument>.Filter.Eq(x => x.Status, "SUSPENDED")
                );
                
                var update = Builders<SuspensionDocument>.Update
                    .Set(x => x.AppealText, appealText)
                    .Set(x => x.AppealDate, DateTime.UtcNow)
                    .Set(x => x.Status, "APPEALED");

                var result = await _suspensions.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<SuspensionDocument> { ReturnDocument = ReturnDocument.After }
                );

                if (result == null)
                {
                    return null;
                }

                _logger.LogInformation("Appeal added to suspension for restaurant {RestaurantId} by owner {OwnerId}", restaurantId, ownerId);

                return new SuspensionResponseDto
                {
                    Id = result.Id.ToString(),
                    RestaurantId = result.RestaurantId,
                    SuspensionReason = result.SuspensionReason,
                    SuspendedAt = result.SuspendedAt,
                    SuspendedBy = result.SuspendedBy,
                    Status = result.Status,
                    AppealText = result.AppealText,
                    AppealDate = result.AppealDate,
                    DecisionDate = result.DecisionDate,
                    DecisionBy = result.DecisionBy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating suspension with appeal for restaurant {RestaurantId}", restaurantId);
                throw;
            }
        }

        public async Task<SuspensionResponseDto?> UpdateSuspensionDecisionAsync(int restaurantId, bool accept, int adminId)
        {
            try
            {
                var filter = Builders<SuspensionDocument>.Filter.And(
                    Builders<SuspensionDocument>.Filter.Eq(x => x.RestaurantId, restaurantId),
                    Builders<SuspensionDocument>.Filter.In(x => x.Status, new[] { "APPEALED", "REJECTED" })
                );
                
                var update = Builders<SuspensionDocument>.Update
                    .Set(x => x.Status, accept ? "ACCEPTED" : "REJECTED")
                    .Set(x => x.DecisionDate, DateTime.UtcNow)
                    .Set(x => x.DecisionBy, adminId);

                var result = await _suspensions.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<SuspensionDocument> { ReturnDocument = ReturnDocument.After }
                );

                if (result == null)
                {
                    _logger.LogWarning("No APPEALED or REJECTED suspension found for restaurant {RestaurantId} when updating decision", restaurantId);
                    return null;
                }

                _logger.LogInformation("Suspension decision updated for restaurant {RestaurantId} by admin {AdminId}: {Decision}", 
                    restaurantId, adminId, accept ? "ACCEPTED" : "REJECTED");

                return new SuspensionResponseDto
                {
                    Id = result.Id.ToString(),
                    RestaurantId = result.RestaurantId,
                    SuspensionReason = result.SuspensionReason,
                    SuspendedAt = result.SuspendedAt,
                    SuspendedBy = result.SuspendedBy,
                    Status = result.Status,
                    AppealText = result.AppealText,
                    AppealDate = result.AppealDate,
                    DecisionDate = result.DecisionDate,
                    DecisionBy = result.DecisionBy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating suspension decision for restaurant {RestaurantId}", restaurantId);
                throw;
            }
        }

        public async Task<bool> DeleteSuspensionAsync(int restaurantId)
        {
            try
            {
                var filter = Builders<SuspensionDocument>.Filter.Eq(x => x.RestaurantId, restaurantId);
                var result = await _suspensions.DeleteOneAsync(filter);
                
                _logger.LogInformation("Suspension deleted for restaurant {RestaurantId}", restaurantId);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting suspension for restaurant {RestaurantId}", restaurantId);
                throw;
            }
        }

        public async Task<List<SuspensionResponseDto>> GetAppealedSuspensionsAsync()
        {
            try
            {
                var filter = Builders<SuspensionDocument>.Filter.And(
                    Builders<SuspensionDocument>.Filter.In(x => x.Status, new[] { "APPEALED", "REJECTED" }),
                    Builders<SuspensionDocument>.Filter.Ne(x => x.AppealText, (string?)null)
                );
                
                var suspensions = await _suspensions.Find(filter)
                    .SortByDescending(x => x.AppealDate)
                    .ToListAsync();

                return suspensions.Select(s => new SuspensionResponseDto
                {
                    Id = s.Id.ToString(),
                    RestaurantId = s.RestaurantId,
                    SuspensionReason = s.SuspensionReason,
                    SuspendedAt = s.SuspendedAt,
                    SuspendedBy = s.SuspendedBy,
                    Status = s.Status,
                    AppealText = s.AppealText,
                    AppealDate = s.AppealDate,
                    DecisionDate = s.DecisionDate,
                    DecisionBy = s.DecisionBy
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appealed suspensions");
                throw;
            }
        }

        public async Task<SuspensionResponseDto?> GetAppealedSuspensionByRestaurantIdAsync(int restaurantId)
        {
            try
            {
                var filter = Builders<SuspensionDocument>.Filter.And(
                    Builders<SuspensionDocument>.Filter.Eq(x => x.RestaurantId, restaurantId),
                    Builders<SuspensionDocument>.Filter.In(x => x.Status, new[] { "APPEALED", "REJECTED" }),
                    Builders<SuspensionDocument>.Filter.Ne(x => x.AppealText, (string?)null)
                );
                
                var suspension = await _suspensions.Find(filter)
                    .SortByDescending(x => x.AppealDate)
                    .FirstOrDefaultAsync();
                
                if (suspension == null)
                {
                    return null;
                }
                
                return new SuspensionResponseDto
                {
                    Id = suspension.Id.ToString(),
                    RestaurantId = suspension.RestaurantId,
                    SuspensionReason = suspension.SuspensionReason,
                    SuspendedAt = suspension.SuspendedAt,
                    SuspendedBy = suspension.SuspendedBy,
                    Status = suspension.Status,
                    AppealText = suspension.AppealText,
                    AppealDate = suspension.AppealDate,
                    DecisionDate = suspension.DecisionDate,
                    DecisionBy = suspension.DecisionBy
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appealed suspension for restaurant {RestaurantId}", restaurantId);
                throw;
            }
        }
    }
}

