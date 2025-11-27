using System.Linq;
using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Gozba_na_klik.Services.EmailServices;

namespace Gozba_na_klik.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly GozbaNaKlikDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly ISuspensionRepository _suspensionRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public RestaurantService(
            IRestaurantRepository restaurantRepository, 
            GozbaNaKlikDbContext context, 
            IMapper mapper, 
            ILogger<RestaurantService> logger,
            ISuspensionRepository suspensionRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _restaurantRepository = restaurantRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _suspensionRepository = suspensionRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
        {
            return await _restaurantRepository.GetAllAsync();
        }

        public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
        {
            return await _restaurantRepository.GetByIdAsync(id);
        }

        public async Task<Restaurant> GetRestaurantByIdOrThrowAsync(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null)
            {
                throw new NotFoundException($"Restoran sa ID {id} nije pronađen.");
            }
            return restaurant;
        }

        public async Task<ResponseRestaurantDTO> GetRestaurantDtoByIdAsync(int id)
        {
            var restaurant = await GetRestaurantByIdOrThrowAsync(id);
            var currentDate = DateTime.UtcNow;

            return new ResponseRestaurantDTO
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                PhotoUrl = restaurant.PhotoUrl,
                Address = restaurant.Address,
                Description = restaurant.Description,
                Phone = restaurant.Phone,
                Menu = restaurant.Menu,
                ClosedDates = restaurant.ClosedDates,
                isOpen = IsRestaurantOpen(restaurant, currentDate)
            };
        }

        public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
        {
            return await _restaurantRepository.AddAsync(restaurant);
        }
        public async Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant)
        {
            return await _restaurantRepository.UpdateAsync(restaurant);
        }

        public async Task<Restaurant> UpdateRestaurantByOwnerAsync(int id, RestaurantUpdateDto dto, int ownerId)
        {
            var restaurant = await GetRestaurantByIdOrThrowAsync(id);

            if (restaurant.OwnerId != ownerId)
            {
                throw new UnauthorizedAccessException("Nemate dozvolu da menjate ovaj restoran.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 150)
            {
                throw new BadRequestException("Naziv je obavezan i najviše 150 karaktera.");
            }

            restaurant.Name = dto.Name?.Trim();
            restaurant.Address = dto.Address?.Trim();
            restaurant.Description = dto.Description?.Trim();
            restaurant.Phone = dto.Phone?.Trim();
            restaurant.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(dto.PhotoUrl))
            {
                restaurant.PhotoUrl = dto.PhotoUrl;
            }

            return await _restaurantRepository.UpdateAsync(restaurant);
        }

        // ADMIN Create 
        public async Task<Restaurant> CreateRestaurantByAdminAsync(RequestCreateRestaurantByAdminDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.OwnerId == null)
            {
                throw new BadRequestException("Name and Owner ID cannot be null.");
            }
            _logger.LogInformation("Creating new restaurant");
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedAt = DateTime.UtcNow;
            return await _restaurantRepository.AddAsync(restaurant);
        }

        // ADMIN Update
        public async Task<Restaurant> UpdateRestaurantByAdminAsync(int id, RequestUpdateRestaurantByAdminDto dto)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null)
                return null;

            if (string.IsNullOrWhiteSpace(dto.Name) || dto.OwnerId == null)
            {
                throw new BadRequestException("Name and Owner ID cannot be null.");
            }

            _logger.LogInformation("Updating restaurant");
            restaurant.Name = dto.Name;
            restaurant.OwnerId = dto.OwnerId;
            restaurant.UpdatedAt = DateTime.UtcNow;
            return await _restaurantRepository.UpdateAsync(restaurant);
        }

        public async Task DeleteRestaurantAsync(int id)
        {
            await _restaurantRepository.DeleteAsync(id);
        }

        public async Task<bool> RestaurantExistsAsync(int id)
        {
            return await _restaurantRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsByOwnerAsync(int ownerId)
        {
            return await _restaurantRepository.GetByOwnerAsync(ownerId);
        }

        public async Task UpdateWorkSchedulesFromDtosAsync(int restaurantId, List<DTOs.WorkScheduleDto> scheduleDtos)
        {
            if (scheduleDtos == null || !scheduleDtos.Any())
            {
                throw new BadRequestException("Lista radnih vremena ne može biti prazna.");
            }

            List<WorkSchedule> schedules = new List<WorkSchedule>();
            foreach (var dto in scheduleDtos)
            {
                if (!TimeSpan.TryParse(dto.OpenTime, out TimeSpan openTime))
                {
                    throw new BadRequestException($"Nevažeći format vremena za otvaranje: {dto.OpenTime}");
                }

                if (!TimeSpan.TryParse(dto.CloseTime, out TimeSpan closeTime))
                {
                    throw new BadRequestException($"Nevažeći format vremena za zatvaranje: {dto.CloseTime}");
                }

                if (openTime >= closeTime && closeTime != TimeSpan.Zero)
                {
                    throw new BadRequestException($"Vreme otvaranja ({dto.OpenTime}) mora biti pre vremena zatvaranja ({dto.CloseTime}).");
                }

                schedules.Add(new WorkSchedule
                {
                    DayOfWeek = (DayOfWeek)dto.DayOfWeek,
                    OpenTime = openTime,
                    CloseTime = closeTime
                });
            }

            await UpdateWorkSchedulesAsync(restaurantId, schedules);
        }

        public async Task UpdateWorkSchedulesAsync(int restaurantId, List<WorkSchedule> schedules)
        {
            Restaurant? restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException($"Restoran sa ID {restaurantId} nije pronađen.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingSchedules = await _context.WorkSchedules
                    .Where(ws => ws.RestaurantId == restaurantId)
                    .ToListAsync();

                if (existingSchedules.Any())
                {
                    _context.WorkSchedules.RemoveRange(existingSchedules);
                    await _context.SaveChangesAsync();
                }

                foreach (var scheduleData in schedules)
                {
                    var newSchedule = new WorkSchedule
                    {
                        RestaurantId = restaurantId,
                        DayOfWeek = scheduleData.DayOfWeek,
                        OpenTime = scheduleData.OpenTime,
                        CloseTime = scheduleData.CloseTime
                    };

                    _context.WorkSchedules.Add(newSchedule);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating work schedules for restaurant {RestaurantId}", restaurantId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddClosedDateAsync(int restaurantId, ClosedDate date)
        {
            date.RestaurantId = restaurantId;
            _context.ClosedDates.Add(date);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveClosedDateAsync(int restaurantId, int dateId)
        {
            ClosedDate? closedDate = await _context.ClosedDates
                .FirstOrDefaultAsync(cd => cd.Id == dateId && cd.RestaurantId == restaurantId);

            if (closedDate != null)
            {
                _context.ClosedDates.Remove(closedDate);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<PaginatedList<ResponseRestaurantDTO>> GetAllFilteredSortedPagedAsync(
    RestaurantFilter filter, int sortType, int page, int pageSize)
        {
            var pagedRestaurants = await _restaurantRepository.GetAllFilteredSortedPagedAsync(filter, sortType, page, pageSize);
            var currentDate = filter.CurrentDate ?? DateTime.UtcNow;
            var currentDay = currentDate.DayOfWeek;
            var currentTime = currentDate.TimeOfDay;

            var dtoItems = pagedRestaurants.Items.Select(r => new ResponseRestaurantDTO
            {
                Id = r.Id,
                Name = r.Name,
                PhotoUrl = r.PhotoUrl,
                Address = r.Address,
                Description = r.Description,
                Phone = r.Phone,
                Menu = r.Menu,
                ClosedDates = r.ClosedDates,
                isOpen = IsRestaurantOpen(r, currentDate)

            }).ToList();

            return new PaginatedList<ResponseRestaurantDTO>(
                dtoItems, pagedRestaurants.Count, pagedRestaurants.PageIndex, pageSize);
        }
        public async Task<List<SortTypeOption>> GetSortTypesAsync()  //dobavlja vrste sortiranja
        {
            return await _restaurantRepository.GetSortTypesAsync();
        }
        private bool IsRestaurantOpen(Restaurant r, DateTime currentDate)
        {
            var currentDay = currentDate.DayOfWeek;
            var currentTime = currentDate.TimeOfDay;

            var isClosedDate = r.ClosedDates.Any(cd => cd.Date.Date == currentDate.Date);
            if (isClosedDate)
            {
                _logger.LogInformation("Restoran {RestaurantId} je zatvoren na datum {Date}", r.Id, currentDate.Date);
                return false;
            }

            // Proveri da li postoji radno vreme za trenutni dan
            var todaySchedule = r.WorkSchedules.FirstOrDefault(ws => ws.DayOfWeek == currentDay);
            if (todaySchedule == null)
            {
                _logger.LogInformation("Restoran {RestaurantId} nema radno vreme za dan {DayOfWeek}", r.Id, currentDay);
                return false;
            }

            // Proveri da li je trenutno vreme između vremena otvaranja i zatvaranja
            var isOpen = currentTime >= todaySchedule.OpenTime && currentTime <= todaySchedule.CloseTime;

            if (!isOpen)
            {
                _logger.LogInformation("Restoran {RestaurantId} je zatvoren. Trenutno vreme: {CurrentTime}, Radno vreme: {OpenTime} - {CloseTime}",
                    r.Id, currentTime, todaySchedule.OpenTime, todaySchedule.CloseTime);
            }

            return isOpen;
        }

        public async Task<List<IrresponsibleRestaurantDto>> GetIrresponsibleRestaurantsAsync()
        {
            _logger.LogInformation("Getting irresponsible restaurants");
            var irresponsibleRestaurants = await _restaurantRepository.GetIrresponsibleRestaurantsAsync();
            
            var result = new List<IrresponsibleRestaurantDto>();
            
            foreach (var item in irresponsibleRestaurants)
            {
                var suspension = await _suspensionRepository.GetSuspensionByRestaurantIdAsync(item.Restaurant.Id);
                var isSuspended = suspension != null && 
                    (suspension.Status == "SUSPENDED" || 
                     suspension.Status == "APPEALED" || 
                     suspension.Status == "REJECTED");
                
                result.Add(new IrresponsibleRestaurantDto
                {
                    Id = item.Restaurant.Id,
                    Name = item.Restaurant.Name,
                    Address = item.Restaurant.Address,
                    Phone = item.Restaurant.Phone,
                    OwnerUsername = item.Restaurant.Owner?.UserName,
                    CancelledOrdersCount = item.CancelledCount,
                    IsSuspended = isSuspended,
                    SuspensionStatus = suspension?.Status
                });
            }

            _logger.LogInformation("Successfully retrieved {Count} irresponsible restaurants", result.Count);
            return result;
        }

        public async Task<SuspensionResponseDto> SuspendRestaurantAsync(int restaurantId, string reason, int adminId)
        {
            var restaurant = await GetRestaurantByIdOrThrowAsync(restaurantId);

            if (restaurant.Owner == null)
            {
                await _context.Entry(restaurant)
                    .Reference(r => r.Owner)
                    .LoadAsync();
            }

            var existingSuspension = await _suspensionRepository.GetSuspensionByRestaurantIdAsync(restaurantId);
            if (existingSuspension != null)
            {
                throw new BadRequestException("Restoran je već suspendovan.");
            }

            var suspension = await _suspensionRepository.InsertSuspensionAsync(restaurantId, reason, adminId);
            
            _logger.LogInformation("Restaurant {RestaurantId} suspended by admin {AdminId}", restaurantId, adminId);

            try
            {
                if (restaurant.Owner != null && !string.IsNullOrEmpty(restaurant.Owner.Email))
                {
                    var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
                    var suspensionLink = $"{frontendUrl}/restaurants/{restaurantId}/suspension";
                    
                    var emailBody = $@"
                        <html>
                        <body>
                            <h2>Obaveštenje o suspenziji restorana</h2>
                            <p>Poštovani/a,</p>
                            <p>Vaš restoran <strong>{restaurant.Name}</strong> je suspendovan.</p>
                            <p><strong>Razlog suspenzije:</strong></p>
                            <p style='background-color: #f3f4f6; padding: 1rem; border-radius: 0.5rem;'>{reason}</p>
                            <p>Za više informacija o suspenziji, kliknite na sledeći link:</p>
                            <p><a href='{suspensionLink}' style='background-color: #dc2626; color: white; padding: 0.75rem 1.5rem; text-decoration: none; border-radius: 0.375rem; display: inline-block;'>Pregled suspenzije</a></p>
                            <p>Srdačan pozdrav,<br>Gozba na klik</p>
                        </body>
                        </html>";

                    await _emailService.SendEmailAsync(
                        restaurant.Owner.Email,
                        $"Suspenzija restorana: {restaurant.Name}",
                        emailBody
                    );

                    _logger.LogInformation("Suspension email sent to owner {OwnerEmail} for restaurant {RestaurantId}", 
                        restaurant.Owner.Email, restaurantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send suspension email to owner for restaurant {RestaurantId}", restaurantId);
            }

            return suspension;
        }

        public async Task<SuspensionResponseDto?> GetRestaurantSuspensionAsync(int restaurantId)
        {
            var suspension = await _suspensionRepository.GetSuspensionByRestaurantIdAsync(restaurantId);
            return suspension;
        }

        public async Task<SuspensionResponseDto> AppealSuspensionAsync(int restaurantId, string appealText, int ownerId)
        {
            var restaurant = await GetRestaurantByIdOrThrowAsync(restaurantId);

            if (restaurant.OwnerId != ownerId)
            {
                throw new ForbiddenException("Nemate pravo da podnesete žalbu za ovaj restoran.");
            }

            var existingSuspension = await _suspensionRepository.GetSuspensionByRestaurantIdAsync(restaurantId);
            if (existingSuspension == null)
            {
                throw new NotFoundException("Suspenzija nije pronađena.");
            }

            if (existingSuspension.Status == "APPEALED")
            {
                throw new BadRequestException("Žalba je već podneta za ovu suspenziju.");
            }

            var updatedSuspension = await _suspensionRepository.UpdateSuspensionWithAppealAsync(
                restaurantId, 
                appealText, 
                ownerId
            );

            if (updatedSuspension == null)
            {
                throw new NotFoundException("Greška pri ažuriranju suspenzije sa žalbom. Suspenzija možda nije u statusu SUSPENDED.");
            }

            _logger.LogInformation("Appeal submitted for restaurant {RestaurantId} by owner {OwnerId}", restaurantId, ownerId);

            return updatedSuspension;
        }

        public async Task<List<SuspensionResponseDto>> GetAppealedSuspensionsAsync()
        {
            var appeals = await _suspensionRepository.GetAppealedSuspensionsAsync();
            
            if (appeals.Count == 0)
                return appeals;
            
            var restaurantIds = appeals.Select(a => a.RestaurantId).Distinct().ToList();
            var restaurants = await _context.Restaurants
                .Where(r => restaurantIds.Contains(r.Id))
                .Select(r => new { r.Id, r.Name })
                .AsNoTracking()
                .ToListAsync();
            
            var restaurantDict = restaurants.ToDictionary(r => r.Id, r => r.Name);
            
            foreach (var appeal in appeals)
            {
                if (restaurantDict.TryGetValue(appeal.RestaurantId, out var name))
                {
                    appeal.RestaurantName = name;
                }
            }
            
            return appeals;
        }

        public async Task ProcessAppealDecisionAsync(int restaurantId, bool accept, int adminId)
        {
            var restaurant = await GetRestaurantByIdOrThrowAsync(restaurantId);

            var appeal = await _suspensionRepository.GetAppealedSuspensionByRestaurantIdAsync(restaurantId);
            
            if (appeal == null)
            {
                _logger.LogWarning("No appealed suspension found for restaurant {RestaurantId} when processing appeal decision", restaurantId);
                throw new NotFoundException("Žalba za ovaj restoran nije pronađena.");
            }

            _logger.LogInformation("Processing appeal decision for restaurant {RestaurantId}. Status: {Status}, AppealText: {HasAppealText}", 
                restaurantId, appeal.Status, appeal.AppealText != null ? "Yes" : "No");

            var suspension = appeal;

            var updatedSuspension = await _suspensionRepository.UpdateSuspensionDecisionAsync(restaurantId, accept, adminId);
            if (updatedSuspension == null)
            {
                throw new InvalidOperationException("Greška pri ažuriranju odluke o suspenziji.");
            }
            
            _logger.LogInformation("Appeal {Decision} for restaurant {RestaurantId} by admin {AdminId}", 
                accept ? "accepted" : "rejected", restaurantId, adminId);

            try
            {
                var owner = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == restaurant.OwnerId);

                if (owner != null)
                {
                    var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
                    var decisionText = accept ? "prihvaćena" : "odbijena";
                    var decisionMessage = accept 
                        ? "Vaša žalba je prihvaćena i suspenzija je uklonjena. Restoran je ponovo aktivan."
                        : "Vaša žalba je odbijena. Suspenzija ostaje na snazi.";

                    var emailBody = $@"
                        <html>
                        <body>
                            <h2>Obaveštenje o odluci na žalbu</h2>
                            <p>Poštovani/a {owner.UserName},</p>
                            <p>Vaša žalba na suspenziju restorana <strong>{restaurant.Name}</strong> je {decisionText}.</p>
                            <p><strong>Razlog suspenzije:</strong></p>
                            <p style='background-color: #f3f4f6; padding: 1rem; border-radius: 0.5rem;'>{suspension.SuspensionReason}</p>
                            {(suspension.AppealText != null ? $@"
                            <p><strong>Vaša žalba:</strong></p>
                            <p style='background-color: #f0f9ff; padding: 1rem; border-radius: 0.5rem;'>{suspension.AppealText}</p>
                            " : "")}
                            <p><strong>Odluka:</strong></p>
                            <p style='background-color: {(accept ? "#d1fae5" : "#fee2e2")}; padding: 1rem; border-radius: 0.5rem; font-weight: bold;'>
                                {decisionMessage}
                            </p>
                            <p>Srdačan pozdrav,<br>Gozba na klik</p>
                        </body>
                        </html>";

                    await _emailService.SendEmailAsync(
                        owner.Email,
                        $"Odgovor na žalbu - Restoran: {restaurant.Name}",
                        emailBody
                    );

                    _logger.LogInformation("Appeal decision email sent to owner {OwnerEmail} for restaurant {RestaurantId}", 
                        owner.Email, restaurantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send appeal decision email to owner for restaurant {RestaurantId}", restaurantId);
            }
        }

    }
}