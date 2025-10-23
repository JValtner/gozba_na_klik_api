using System.Text.Json;
using AutoMapper;
using Gozba_na_klik.DTOs.Orders;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Gozba_na_klik.Repositories;
using Microsoft.Extensions.Logging;

namespace Gozba_na_klik.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMealsRepository _mealsRepository;
        private readonly IMealAddonsRepository _addonRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private const decimal DELIVERY_FEE = 200m;

        public OrderService(
            IOrderRepository orderRepository,
            IRestaurantRepository restaurantRepository,
            IMealsRepository mealsRepository,
            IMealAddonsRepository addonRepository,
            IAddressRepository addressRepository,
            IUsersRepository usersRepository,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _restaurantRepository = restaurantRepository;
            _mealsRepository = mealsRepository;
            _addonRepository = addonRepository;
            _addressRepository = addressRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderPreviewDto> GetOrderPreviewAsync(int userId, int restaurantId, CreateOrderDto dto)
        {
            _logger.LogInformation("Creating order preview for user {UserId} at restaurant {RestaurantId}",
                userId, restaurantId);

            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new NotFoundException($"Restoran sa ID {restaurantId} nije pronađen.");

            var isOpen = IsRestaurantOpen(restaurant);
            string? closedReason = null;

            if (!isOpen)
            {
                closedReason = GetClosedReason(restaurant);
            }

            decimal subtotal = 0;
            var itemPreviews = new List<OrderItemPreviewDto>();
            var allergenSet = new HashSet<string>();

            foreach (var itemDto in dto.Items)
            {
                var meal = await _mealsRepository.GetByIdAsync(itemDto.MealId);
                if (meal == null)
                    throw new NotFoundException($"Jelo sa ID {itemDto.MealId} nije pronađeno.");

                if (meal.RestaurantId != restaurantId)
                    throw new BadRequestException($"Jelo {meal.Name} ne pripada restoranu {restaurant.Name}.");

                decimal itemPrice = meal.Price * itemDto.Quantity;
                var selectedAddons = new List<AddonPreviewDto>();

                foreach (var allergen in meal.Alergens)
                {
                    allergenSet.Add(allergen.Name);
                }

                if (itemDto.SelectedAddonIds != null && itemDto.SelectedAddonIds.Any())
                {
                    foreach (var addonId in itemDto.SelectedAddonIds)
                    {
                        var addon = await _addonRepository.GetByIdAsync(addonId);
                        if (addon == null)
                            throw new NotFoundException($"Dodatak sa ID {addonId} nije pronađen.");

                        if (addon.MealId != meal.Id)
                            throw new BadRequestException($"Dodatak ne pripada ovom jelu.");

                        itemPrice += addon.Price * itemDto.Quantity;
                        selectedAddons.Add(new AddonPreviewDto
                        {
                            Id = addon.Id,
                            Name = addon.Name,
                            Price = addon.Price
                        });
                    }
                }

                subtotal += itemPrice;

                itemPreviews.Add(new OrderItemPreviewDto
                {
                    MealId = meal.Id,
                    MealName = meal.Name,
                    MealImagePath = meal.ImagePath,
                    Quantity = itemDto.Quantity,
                    UnitPrice = meal.Price,
                    TotalPrice = itemPrice,
                    SelectedAddons = selectedAddons
                });
            }

            return new OrderPreviewDto
            {
                RestaurantId = restaurantId,
                RestaurantName = restaurant.Name,
                IsRestaurantOpen = isOpen,
                ClosedReason = closedReason,
                Items = itemPreviews,
                SubtotalPrice = subtotal,
                DeliveryFee = DELIVERY_FEE,
                TotalPrice = subtotal + DELIVERY_FEE,
                HasAllergens = allergenSet.Any(),
                Allergens = allergenSet.ToList()
            };
        }

        public async Task<OrderResponseDto> CreateOrderAsync(int userId, int restaurantId, CreateOrderDto dto)
        {
            _logger.LogInformation("Creating order for user {UserId} at restaurant {RestaurantId}",
                userId, restaurantId);

            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);
            if (restaurant == null)
                throw new NotFoundException($"Restoran sa ID {restaurantId} nije pronađen.");

            if (!IsRestaurantOpen(restaurant))
            {
                var reason = GetClosedReason(restaurant);
                throw new BadRequestException($"Restoran je trenutno zatvoren. {reason}");
            }

            var address = await _addressRepository.GetByIdAsync(dto.AddressId);
            if (address == null || address.UserId != userId)
                throw new NotFoundException("Adresa za dostavu nije pronađena ili ne pripada korisniku.");

            decimal subtotal = 0;
            var orderItems = new List<OrderItem>();
            var allergenSet = new HashSet<string>();

            foreach (var itemDto in dto.Items)
            {
                var meal = await _mealsRepository.GetByIdAsync(itemDto.MealId);
                if (meal == null)
                    throw new NotFoundException($"Jelo sa ID {itemDto.MealId} nije pronađeno.");

                if (meal.RestaurantId != restaurantId)
                    throw new BadRequestException($"Jelo {meal.Name} ne pripada restoranu {restaurant.Name}.");

                decimal itemPrice = meal.Price * itemDto.Quantity;
                var addonNames = new List<string>();

                foreach (var allergen in meal.Alergens)
                {
                    allergenSet.Add(allergen.Name);
                }

                if (itemDto.SelectedAddonIds != null && itemDto.SelectedAddonIds.Any())
                {
                    foreach (var addonId in itemDto.SelectedAddonIds)
                    {
                        var addon = await _addonRepository.GetByIdAsync(addonId);
                        if (addon == null)
                            throw new NotFoundException($"Dodatak sa ID {addonId} nije pronađen.");

                        if (addon.MealId != meal.Id)
                            throw new BadRequestException($"Dodatak ne pripada ovom jelu.");

                        itemPrice += addon.Price * itemDto.Quantity;
                        addonNames.Add(addon.Name);
                    }
                }

                subtotal += itemPrice;

                orderItems.Add(new OrderItem
                {
                    MealId = itemDto.MealId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = meal.Price,
                    TotalPrice = itemPrice,
                    SelectedAddons = addonNames.Any() ? JsonSerializer.Serialize(addonNames) : null
                });
            }

            if (allergenSet.Any() && !dto.AllergenWarningAccepted)
            {
                throw new BadRequestException(
                    $"Ova porudžbina sadrži alergene: {string.Join(", ", allergenSet)}. " +
                    "Morate prihvatiti upozorenje o alergenima.");
            }

            var totalPrice = subtotal + DELIVERY_FEE;

            var order = new Order
            {
                UserId = userId,
                RestaurantId = restaurantId,
                AddressId = dto.AddressId,
                Status = OrderStatus.NA_CEKANJU,
                OrderDate = DateTime.UtcNow,
                SubtotalPrice = subtotal,
                DeliveryFee = DELIVERY_FEE,
                TotalPrice = totalPrice,
                CustomerNote = dto.CustomerNote,
                HasAllergenWarning = allergenSet.Any(),
                Items = orderItems
            };

            var created = await _orderRepository.AddAsync(order);

            // ✅ DODAJ OVE 3 LINIJE ZA DEBUG
            _logger.LogInformation("Order created. Items count: {ItemsCount}. First item Meal is null: {MealIsNull}",
                created.Items?.Count ?? 0,
                created.Items?.FirstOrDefault()?.Meal == null);

            _logger.LogInformation("Order {OrderId} created successfully with total price {TotalPrice}",
                created.Id, totalPrice);

            return _mapper.Map<OrderResponseDto>(created);
        }

        private bool IsRestaurantOpen(Restaurant restaurant)
        {
            var now = DateTime.UtcNow;
            var today = now.DayOfWeek;
            var currentTime = now.TimeOfDay;

            if (restaurant.ClosedDates.Any(cd => cd.Date.Date == now.Date))
            {
                return false;
            }

            var schedule = restaurant.WorkSchedules.FirstOrDefault(ws => ws.DayOfWeek == today);
            if (schedule == null)
            {
                return false;
            }

            return currentTime >= schedule.OpenTime && currentTime <= schedule.CloseTime;
        }

        private string GetClosedReason(Restaurant restaurant)
        {
            var now = DateTime.UtcNow;
            var closedDate = restaurant.ClosedDates.FirstOrDefault(cd => cd.Date.Date == now.Date);

            if (closedDate != null)
            {
                return string.IsNullOrEmpty(closedDate.Reason)
                    ? "Restoran je zatvoren danas."
                    : $"Restoran je zatvoren danas. Razlog: {closedDate.Reason}";
            }

            var schedule = restaurant.WorkSchedules.FirstOrDefault(ws => ws.DayOfWeek == now.DayOfWeek);
            if (schedule == null)
            {
                return "Restoran ne radi danas.";
            }

            return $"Restoran radi od {schedule.OpenTime:hh\\:mm} do {schedule.CloseTime:hh\\:mm}.";
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