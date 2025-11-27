using System.Text.Json;
using AutoMapper;
using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Hubs;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly GozbaNaKlikDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IInvoiceService _invoiceService;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<CourierLocationHub> _hub;
        private readonly ISuspensionRepository _suspensionRepository;

        public OrderService(
            IOrderRepository orderRepository,
            GozbaNaKlikDbContext context,
            IMapper mapper,
            ILogger<OrderService> logger,
            IInvoiceService invoiceService,
            UserManager<User> userManager,
            IHubContext<CourierLocationHub> hub,
            ISuspensionRepository suspensionRepository)
        {
            _orderRepository = orderRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _invoiceService = invoiceService;
            _userManager = userManager;
            _hub = hub;
            _suspensionRepository = suspensionRepository;
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

            var suspension = await _suspensionRepository.GetSuspensionByRestaurantIdAsync(restaurantId);
            if (suspension != null && (suspension.Status == "SUSPENDED" || suspension.Status == "APPEALED"))
            {
                throw new BadRequestException("Restoran suspendovan.");
            }

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

            var suspension = await _suspensionRepository.GetSuspensionByRestaurantIdAsync(restaurantId);
            if (suspension != null && (suspension.Status == "SUSPENDED" || suspension.Status == "APPEALED"))
            {
                throw new BadRequestException("Restoran suspendovan.");
            }

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

            var isOrderOwner = order.UserId == userId;
            var isRestaurantOwner = order.Restaurant != null && order.Restaurant.OwnerId == userId;

            if (!isOrderOwner && !isRestaurantOwner)
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
                CustomerName = order.User?.UserName ?? "N/A",
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
                CustomerName = order.User?.UserName ?? "N/A",
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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == order.RestaurantId);

            if (user == null)
                throw new NotFoundException("Korisnik nije pronađen.");
            if (restaurant == null)
                throw new NotFoundException("Restoran nije pronađen.");

            // ✅ Fetch role from Identity
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            bool isOwner = restaurant.OwnerId == user.Id;
            bool isEmployee = user.RestaurantId == restaurant.Id && role == "RestaurantEmployee";

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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == order.RestaurantId);

            if (user == null)
                throw new NotFoundException("Korisnik nije pronađen.");
            if (restaurant == null)
                throw new NotFoundException("Restoran nije pronađen.");

            // ✅ Fetch role from Identity
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            bool isOwner = restaurant.OwnerId == user.Id;
            bool isEmployee = user.RestaurantId == restaurant.Id && role == "RestaurantEmployee";

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

        public async Task<PaginatedOrderHistoryResponseDto> GetUserOrderHistoryAsync(int userId, int requestingUserId, string? statusFilter, int page, int pageSize)
        {

            if (userId != requestingUserId)
            {
                _logger.LogWarning("User {RequestingUserId} attempted to access orders of user {UserId}", requestingUserId, userId);
                throw new ForbiddenException("Možete videti samo svoje porudžbine.");
            }

            // Validacija: Page i pageSize
            if (page < 1)
                page = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

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
        // 1 DOHVATI SVE PRIHVACENE
        public async Task<List<Order>> GetAllAcceptedOrdersAsync()
        {
            return await _orderRepository.GetAllAcceptedOrdersAsync();
        }

        // DODELI DOSTAVU DOSTAVLJACU
        public async Task AssignCourierToOrderAsync(int orderId, int courierId)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(orderId);
            if (existingOrder == null)
                throw new NotFoundException($"Porudzbina sa ID-em {orderId} nije pronadjena.");

            _logger.LogInformation("Menjam status dostave iz 'PRIHVACENA' u 'PREUZIMANJE U TOKU'.");
            existingOrder.DeliveryPersonId = courierId;
            existingOrder.Status = "PREUZIMANJE U TOKU";
            await _orderRepository.UpdateAsync(existingOrder);
        }

        //  DA LI IMA DODELJENA DOSTAVA?
        // Dohvati dostavu koja ima dostavljaca i u preuzimanju
        public async Task<CourierActiveOrderDto?> GetCourierOrderInPickupAsync(int courierId)
        {
            var order = await _orderRepository.GetCourierOrderInPickupAsync(courierId);
            if (order == null)
            {
                return null;
            }
            return _mapper.Map<CourierActiveOrderDto>(order);
        }

        // DOSTAVA U TOKU 
        // Dodeli dostavi status "DOSTAVA U TOKU"
        public async Task<OrderStatusDto?> UpdateOrderToInDeliveryAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Porudzbina sa ID-em {orderId} nije pronadjena.");

            _logger.LogInformation("Menjam status dostave iz 'PREUZIMANJE U TOKU' u 'DOSTAVA U TOKU'.");
            order.Status = "DOSTAVA U TOKU";
            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderStatusDto>(order);
        }

        // DOSTAVA SE ZAVRSAVA
        // Dodeli dostavu status "ZAVRSENO"
        public async Task<OrderStatusDto?> UpdateOrderToDeliveredAsync(int orderId)
        {
            _logger.LogInformation("Trazim dostavu po id iz repoa");
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Porudzbina sa ID-em {orderId} nije pronadjena.");

            _logger.LogInformation("Menjam status dostave u ZAVRŠENO");
            order.Status = "ZAVRŠENO";
            var courierId = order.DeliveryPersonId;
            await _orderRepository.UpdateAsync(order);
            await _hub.Clients.Group(orderId.ToString()).SendAsync("OrderCompleted");

            // Kreiranje Invoice-a
            try
            {
                _logger.LogInformation("Starting automatic invoice generation for completed order {OrderId}", orderId);
                var invoice = await _invoiceService.GenerateInvoiceAsync(orderId);
                await _invoiceService.SaveInvoiceAsync(invoice);
                _logger.LogInformation("Invoice {InvoiceId} automatically created for order {OrderId}", invoice.InvoiceId, orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate invoice for order {OrderId}. Order completed successfully, but invoice creation failed.", orderId);
            }

            if (courierId.HasValue)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == courierId.Value);
                if (user != null)
                {
                    _logger.LogInformation("Skidam dostavu sa dostavljaca");
                    user.ActiveOrderId = null;
                    await _context.SaveChangesAsync();
                }
            }

            return _mapper.Map<OrderStatusDto>(order);
        }

        // Dobavi aktivnu dostavu za Korisnika
        public async Task<OrderStatusResponseDto> GetActiveOrderStatusAsync(int userId)
        {
            var activeOrder = await _orderRepository.GetActiveOrderStatusAsync(userId);

            if (activeOrder == null)
            {
                _logger.LogInformation("Korisnik pod ID-em {UserId} nema aktivnu porudzbinu.", userId);
                throw new NotFoundException(userId);
            }

            return _mapper.Map<OrderStatusResponseDto>(activeOrder);
        }

    }
}