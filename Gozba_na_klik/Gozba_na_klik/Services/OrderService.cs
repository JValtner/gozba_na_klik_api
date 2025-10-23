using AutoMapper;
using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Gozba_na_klik.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Gozba_na_klik.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUsersRepository _userRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly GozbaNaKlikDbContext _context;
        private readonly IMealsRepository _mealsRepository;
        private readonly IMealAddonsRepository _addonRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IUsersRepository userRepository,
            IRestaurantRepository restaurantRepository,
            GozbaNaKlikDbContext context,
            IMealsRepository mealsRepository,
            IMealAddonsRepository addonRepository,
            IAddressRepository addressRepository,
            IUsersRepository usersRepository,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _restaurantRepository = restaurantRepository;
            _context = context;
            _mealsRepository = mealsRepository;
            _addonRepository = addonRepository;
            _addressRepository = addressRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderPreviewDto> GetOrderPreviewAsync(int userId, int restaurantId, CreateOrderDto dto)
        {
            _logger.LogInformation("Creating order preview for user {UserId} at restaurant {RestaurantId}", userId, restaurantId);

            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserAlergens)
                    .ThenInclude(ua => ua.Alergen)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new NotFoundException("Korisnik nije pronađen.");

            var restaurant = await _context.Restaurants
                .AsNoTracking()
                .Include(r => r.WorkSchedules)
                .Include(r => r.ClosedDates)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
                throw new NotFoundException("Restoran nije pronađen.");

            var now = DateTime.Now;
            var today = now.DayOfWeek;
            var currentTime = now.TimeOfDay;

            var todaySchedule = restaurant.WorkSchedules.FirstOrDefault(ws => ws.DayOfWeek == today);
            bool isOpen = todaySchedule != null &&
                         currentTime >= todaySchedule.OpenTime &&
                         currentTime <= todaySchedule.CloseTime;

            var isClosed = restaurant.ClosedDates.Any(cd => cd.Date.Date == now.Date);
            string? closedReason = null;

            if (isClosed)
            {
                isOpen = false;
                var closedDate = restaurant.ClosedDates.FirstOrDefault(cd => cd.Date.Date == now.Date);
                closedReason = closedDate?.Reason ?? "Restoran je zatvoren danas.";
            }
            else if (!isOpen && todaySchedule == null)
            {
                closedReason = "Restoran ne radi danas.";
            }
            else if (!isOpen)
            {
                closedReason = $"Restoran radi od {todaySchedule!.OpenTime:hh\\:mm} do {todaySchedule.CloseTime:hh\\:mm}.";
            }

            var address = await _context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == dto.AddressId && a.UserId == userId && a.IsActive);

            if (address == null)
                throw new NotFoundException("Adresa nije pronađena ili ne pripada korisniku.");

            var itemPreviews = new List<OrderItemPreviewDto>();
            var allergenSet = new HashSet<string>();
            decimal subtotal = 0;

            foreach (var item in dto.Items)
            {
                var meal = await _context.Meals
                    .AsNoTracking()
                    .Include(m => m.Addons)
                    .Include(m => m.Alergens)
                    .FirstOrDefaultAsync(m => m.Id == item.MealId);

                if (meal == null)
                    throw new NotFoundException($"Jelo sa ID {item.MealId} nije pronađeno.");

                decimal itemPrice = meal.Price;
                var selectedAddons = new List<AddonPreviewDto>();

                if (item.SelectedAddonIds != null && item.SelectedAddonIds.Any())
                {
                    var addons = meal.Addons.Where(a => item.SelectedAddonIds.Contains(a.Id)).ToList();
                    foreach (var addon in addons)
                    {
                        itemPrice += addon.Price;
                        selectedAddons.Add(new AddonPreviewDto
                        {
                            Id = addon.Id,
                            Name = addon.Name,
                            Price = addon.Price
                        });
                    }
                }

                decimal itemTotal = itemPrice * item.Quantity;
                subtotal += itemTotal;

                foreach (var alergen in meal.Alergens)
                {
                    allergenSet.Add(alergen.Name);
                }

                itemPreviews.Add(new OrderItemPreviewDto
                {
                    MealId = meal.Id,
                    MealName = meal.Name,
                    MealImagePath = meal.ImagePath,
                    Quantity = item.Quantity,
                    UnitPrice = meal.Price,
                    TotalPrice = itemTotal,
                    SelectedAddons = selectedAddons
                });
            }

            bool hasUserAllergens = user.UserAlergens.Any(ua =>
                allergenSet.Contains(ua.Alergen.Name));

            var preview = new OrderPreviewDto
            {
                RestaurantId = restaurantId,
                RestaurantName = restaurant.Name,
                IsRestaurantOpen = isOpen,
                ClosedReason = closedReason,
                Items = itemPreviews,
                SubtotalPrice = subtotal,
                DeliveryFee = 200m,
                TotalPrice = subtotal + 200m,
                HasAllergens = hasUserAllergens,
                Allergens = allergenSet.ToList()
            };

            _logger.LogInformation("Order preview created successfully. Total: {Total}, Has allergens: {HasAllergens}",
                preview.TotalPrice, preview.HasAllergens);

            return preview;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(int userId, int restaurantId, CreateOrderDto dto)
        {
            _context.ChangeTracker.Clear();

            var userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == userId);

            if (!userExists)
                throw new NotFoundException("Korisnik nije pronađen.");

            var restaurantExists = await _context.Restaurants
                .AsNoTracking()
                .AnyAsync(r => r.Id == restaurantId);

            if (!restaurantExists)
                throw new NotFoundException("Restoran nije pronađen.");

            var addressExists = await _context.Addresses
                .AsNoTracking()
                .AnyAsync(a => a.Id == dto.AddressId && a.UserId == userId && a.IsActive);

            if (!addressExists)
                throw new NotFoundException("Adresa nije pronađena ili ne pripada korisniku.");

            var orderItems = new List<OrderItem>();
            decimal subtotal = 0;

            foreach (var item in dto.Items)
            {
                var meal = await _context.Meals
                    .AsNoTracking()
                    .Include(m => m.Addons)
                    .FirstOrDefaultAsync(m => m.Id == item.MealId);

                if (meal == null)
                    throw new NotFoundException($"Jelo sa ID {item.MealId} nije pronađeno.");

                decimal itemPrice = meal.Price;
                var selectedAddonNames = new List<string>();

                if (item.SelectedAddonIds != null && item.SelectedAddonIds.Any())
                {
                    var addons = meal.Addons.Where(a => item.SelectedAddonIds.Contains(a.Id)).ToList();
                    foreach (var addon in addons)
                    {
                        itemPrice += addon.Price;
                        selectedAddonNames.Add(addon.Name);
                    }
                }

                decimal itemTotal = itemPrice * item.Quantity;
                subtotal += itemTotal;

                orderItems.Add(new OrderItem
                {
                    MealId = meal.Id,
                    Quantity = item.Quantity,
                    UnitPrice = meal.Price,
                    TotalPrice = itemTotal,
                    SelectedAddons = selectedAddonNames.Any()
                        ? JsonSerializer.Serialize(selectedAddonNames)
                        : null
                });
            }

            var order = new Order
            {
                UserId = userId,
                RestaurantId = restaurantId,
                AddressId = dto.AddressId,
                Status = OrderStatus.NA_CEKANJU,
                OrderDate = DateTime.UtcNow,
                SubtotalPrice = subtotal,
                DeliveryFee = 200m,
                TotalPrice = subtotal + 200m,
                CustomerNote = dto.CustomerNote,
                HasAllergenWarning = dto.AllergenWarningAccepted,
                Items = orderItems
            };

            var createdOrder = await _orderRepository.AddAsync(order);

            return _mapper.Map<OrderResponseDto>(createdOrder);
        }

        public async Task<OrderDetailsDto> GetOrderByIdAsync(int userId, int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Porudžbina sa ID {orderId} nije pronađena.");

            if (order.UserId != userId)
                throw new ForbiddenException("Nemate pristup ovoj porudžbini.");

            var dto = new OrderDetailsDto
            {
                Id = order.Id,
                Status = order.Status,
                OrderDate = order.OrderDate,
                RestaurantName = order.Restaurant?.Name ?? "N/A",
                RestaurantId = order.RestaurantId,
                DeliveryAddress = order.Address != null
                    ? $"{order.Address.Street}, {order.Address.City}, {order.Address.PostalCode}"
                    : "N/A",
                CustomerNote = order.CustomerNote,
                HasAllergenWarning = order.HasAllergenWarning,
                Items = order.Items.Select(item => new OrderItemResponseDto
                {
                    Id = item.Id,
                    MealId = item.MealId,
                    MealName = item.Meal?.Name ?? "Unknown",
                    MealImagePath = item.Meal?.ImagePath,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    SelectedAddons = !string.IsNullOrEmpty(item.SelectedAddons)
                        ? JsonSerializer.Deserialize<List<string>>(item.SelectedAddons)
                        : new List<string>()
                }).ToList(),
                SubtotalPrice = order.SubtotalPrice,
                DeliveryFee = order.DeliveryFee,
                TotalPrice = order.TotalPrice
            };

            return dto;
        }

        public async Task<List<RestaurantOrderDto>> GetRestaurantOrdersAsync(int userId, int restaurantId, string? status = null)
        {
            var restaurant = await _context.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
                throw new NotFoundException("Restoran nije pronađen.");

            if (restaurant.OwnerId != userId)
                throw new ForbiddenException("Nemate pristup ovom restoranu.");

            var orders = await _orderRepository.GetRestaurantOrdersAsync(restaurantId, status);

            var dtos = orders.Select(order => new RestaurantOrderDto
            {
                Id = order.Id,
                Status = order.Status,
                OrderDate = order.OrderDate,
                CustomerName = order.User?.Username ?? "N/A",
                CustomerEmail = order.User?.Email,
                DeliveryAddress = order.Address != null
                    ? $"{order.Address.Street}, {order.Address.City}"
                    : "N/A",
                CustomerNote = order.CustomerNote,
                ItemsCount = order.Items.Count,
                TotalPrice = order.TotalPrice,
                Items = order.Items.Select(item => new RestaurantOrderItemDto
                {
                    MealName = item.Meal?.Name ?? "Unknown",
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    SelectedAddons = !string.IsNullOrEmpty(item.SelectedAddons)
                        ? JsonSerializer.Deserialize<List<string>>(item.SelectedAddons)
                        : null
                }).ToList(),
                AcceptedAt = order.AcceptedAt,
                EstimatedPreparationMinutes = order.EstimatedPreparationMinutes
            }).ToList();

            return dtos;
        }

        public async Task AcceptOrderAsync(int userId, int orderId, AcceptOrderDto dto)
        {
            _logger.LogInformation("Accepting order {OrderId} by user {UserId}", orderId, userId);

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Porudžbina sa ID {orderId} nije pronađena.");


            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            var restaurant = await _context.Restaurants.AsNoTracking().FirstOrDefaultAsync(r => r.Id == order.RestaurantId);

            if (user == null)
                throw new NotFoundException("Korisnik nije pronađen.");

            if (restaurant == null)
                throw new NotFoundException("Restoran nije pronađen.");

            bool isOwner = restaurant.OwnerId == user.Id;
            bool isEmployee = user.RestaurantId == restaurant.Id && user.Role == "RestaurantEmployee";

            if (!isOwner && !isEmployee)
                throw new ForbiddenException("Nemate pristup ovoj porudžbini.");

            if (!user.IsActive)
                throw new ForbiddenException("Vaš nalog nije aktivan.");

            if (order.Status != OrderStatus.NA_CEKANJU)
                throw new BadRequestException("Porudžbina se ne može prihvatiti jer nije u statusu NA_CEKANJU.");

            order.Status = OrderStatus.PRIHVAĆENA;
            order.AcceptedAt = DateTime.UtcNow;
            order.EstimatedPreparationMinutes = dto.EstimatedPreparationMinutes;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task CancelOrderAsync(int userId, int orderId, CancelOrderDto dto)
        {
            _logger.LogInformation("Cancelling order {OrderId} by user {UserId}", orderId, userId);

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Porudžbina sa ID {orderId} nije pronađena.");

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            var restaurant = await _context.Restaurants.AsNoTracking().FirstOrDefaultAsync(r => r.Id == order.RestaurantId);

            if (user == null)
                throw new NotFoundException("Korisnik nije pronađen.");

            if (restaurant == null)
                throw new NotFoundException("Restoran nije pronađen.");

            bool isOwner = restaurant.OwnerId == user.Id;
            bool isEmployee = user.RestaurantId == restaurant.Id && user.Role == "RestaurantEmployee";

            if (!isOwner && !isEmployee)
                throw new ForbiddenException("Nemate pristup ovoj porudžbini.");

            if (!user.IsActive)
                throw new ForbiddenException("Vaš nalog nije aktivan.");

            if (order.Status == OrderStatus.ISPORUČENA || order.Status == OrderStatus.OTKAZANA)
                throw new BadRequestException("Ova porudžbina ne može biti otkazana.");

            order.Status = OrderStatus.OTKAZANA;
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = dto.Reason;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task<PaginatedOrderHistoryResponseDto> GetUserOrderHistoryAsync(int userId, string? statusFilter, int page, int pageSize)
        {
            _logger.LogInformation("Getting order history for user {UserId} with filter {StatusFilter}, page {Page}, pageSize {PageSize}",
                userId, statusFilter, page, pageSize);

            var (orders, totalCount) = await _orderRepository.GetOrdersByUserIdAsync(userId, statusFilter, page, pageSize);

            var orderDtos = _mapper.Map<List<OrderHistoryResponseDto>>(orders);

            return new PaginatedOrderHistoryResponseDto
            {
                Orders = orderDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        //  DA LI IMA DODELJENA DOSTAVA?
        // Dohvati dostavu koja ima dostavljaca i u preuzimanju
        public async Task<CourierActiveOrderDto?> GetCourierOrderInPickupAsync(int courierId)
        {
            var order = await _orderRepository.GetCourierOrderInPickupAsync(courierId);
            if (order == null) return null;

            return _mapper.Map<CourierActiveOrderDto>(order);
        }


        // DOSTAVA U TOKU 
        // Dodeli dostavi status "DOSTAVA U TOKU"
        public async Task<OrderStatusDto?> UpdateOrderToInDeliveryAsync(int orderId)
        {
            _logger.LogInformation("Trazim dostavu po id iz repoa");
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }
            order.Status = "DOSTAVA U TOKU";
            _logger.LogInformation("Menjam status dostave u DOSTAVA U TOKU");
            var updatedOrder = await _orderRepository.UpdateOrderStatusAsync(order);

            return _mapper.Map<OrderStatusDto>(updatedOrder);
        }

        // DOSTAVA SE ZAVRSAVA
        // Dodeli dostavu status "ZAVRSENO"
        public async Task<OrderStatusDto?> UpdateOrderToDeliveredAsync(int orderId)
        {
            _logger.LogInformation("Trazim dostavu po id iz repoa");
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }

            var courierId = order.DeliveryPersonId;

            _logger.LogInformation("Menjam status dostave u ZAVRŠENO");
            order.Status = "ZAVRŠENO";
            order.DeliveryPersonId = null;
            var updatedOrder = await _orderRepository.UpdateOrderStatusAsync(order);

            if (courierId.HasValue)
            {
                var user = await _usersRepository.GetByIdAsync(courierId.Value);
                if (user != null)
                {
                    _logger.LogInformation("Skidam dostavu sa dostavljaca");
                    await _usersRepository.ReleaseOrderFromCourierAsync(user);
                }
            }

            return _mapper.Map<OrderStatusDto>(updatedOrder);
        }

    }
}
