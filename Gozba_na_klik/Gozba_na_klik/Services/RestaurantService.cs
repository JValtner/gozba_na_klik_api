using System.Linq;
using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly GozbaNaKlikDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;

        public RestaurantService(IRestaurantRepository restaurantRepository, GozbaNaKlikDbContext context, IMapper mapper, ILogger<RestaurantService> logger)
        {
            _restaurantRepository = restaurantRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
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
        private static bool IsRestaurantOpen(Restaurant r, DateTime currentDate)
        {
            var currentDay = currentDate.DayOfWeek;
            var currentTime = currentDate.TimeOfDay;

            return !r.ClosedDates.Any(cd => cd.Date.Date == currentDate.Date) &&
                   r.WorkSchedules.Any(ws =>
                       ws.DayOfWeek == currentDay &&
                       ws.OpenTime <= currentTime &&
                       ws.CloseTime >= currentTime);
        }

    }
}