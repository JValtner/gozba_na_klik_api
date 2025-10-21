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

        public async Task<Restaurant> CreateRestaurantAsync(Restaurant restaurant)
        {
            return await _restaurantRepository.AddAsync(restaurant);
        }
        public async Task<Restaurant> UpdateRestaurantAsync(Restaurant restaurant)
        {
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

        public async Task UpdateWorkSchedulesAsync(int restaurantId, List<WorkSchedule> schedules)
        {
            Restaurant? restaurant = await _context.Restaurants
                .Include(r => r.WorkSchedules)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null) return;

            _context.WorkSchedules.RemoveRange(restaurant.WorkSchedules);

            foreach (WorkSchedule schedule in schedules)
            {
                schedule.RestaurantId = restaurantId;
                _context.WorkSchedules.Add(schedule);
            }

            await _context.SaveChangesAsync();
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