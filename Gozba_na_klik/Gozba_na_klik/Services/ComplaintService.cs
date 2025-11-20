using Gozba_na_klik.DTOs.Complaints;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Gozba_na_klik.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _complaintRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRestaurantService _restaurantService;
        private readonly GozbaNaKlikDbContext _context;
        private readonly ILogger<ComplaintService> _logger;

        public ComplaintService(
            IComplaintRepository complaintRepository,
            IOrderRepository orderRepository,
            IRestaurantService restaurantService,
            GozbaNaKlikDbContext context,
            ILogger<ComplaintService> logger)
        {
            _complaintRepository = complaintRepository;
            _orderRepository = orderRepository;
            _restaurantService = restaurantService;
            _context = context;
            _logger = logger;
        }

        public async Task<ComplaintResponseDto> CreateComplaintAsync(CreateComplaintDto dto, int userId)
        {
            // Validacija: Proveri da li porudžbina postoji
            var order = await _orderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
            {
                throw new NotFoundException($"Porudžbina sa ID {dto.OrderId} nije pronađena.");
            }

            // Validacija: Proveri da li porudžbina pripada korisniku
            if (order.UserId != userId)
            {
                throw new ForbiddenException("Nemate pravo da podnesete žalbu za ovu porudžbinu.");
            }

            // Validacija: Proveri da li je porudžbina Completed
            var completedStatuses = new[] { "ZAVRŠENO", "ISPORUČENA" };
            if (!completedStatuses.Contains(order.Status))
            {
                throw new BadRequestException("Žalba se može podneti samo za završene porudžbine.");
            }

            // Validacija: Proveri da li već postoji žalba za ovu porudžbinu
            var complaintExists = await _complaintRepository.ComplaintExistsForOrderAsync(dto.OrderId);
            if (complaintExists)
            {
                throw new BadRequestException("Već ste podneli žalbu za ovu porudžbinu.");
            }

            // Dobijanje RestaurantId iz porudžbine
            var restaurantId = order.RestaurantId;

            // Kreiranje žalbe
            var complaint = await _complaintRepository.InsertComplaintAsync(dto, userId, restaurantId);

            _logger.LogInformation("Complaint created for order {OrderId} by user {UserId}", dto.OrderId, userId);

            return complaint;
        }

        public async Task<List<ComplaintResponseDto>> GetComplaintsByRestaurantIdAsync(int restaurantId)
        {
            var complaints = await _complaintRepository.GetComplaintsByRestaurantIdAsync(restaurantId);
            _logger.LogInformation("Retrieved {Count} complaints for restaurant {RestaurantId}", complaints.Count, restaurantId);
            return complaints;
        }

        public async Task<bool> HasComplaintForOrderAsync(int orderId, int userId)
        {
            var hasComplaint = await _complaintRepository.HasComplaintForOrderAsync(orderId, userId);
            _logger.LogInformation("Complaint check for order {OrderId} by user {UserId}: {HasComplaint}", orderId, userId, hasComplaint);
            return hasComplaint;
        }

        public async Task<List<ComplaintResponseDto>> GetComplaintsByOwnerIdAsync(int ownerId)
        {
            // Dobij sve restorane za vlasnika
            var restaurants = await _restaurantService.GetRestaurantsByOwnerAsync(ownerId);
            var restaurantIds = restaurants.Select(r => r.Id).ToList();

            if (!restaurantIds.Any())
            {
                _logger.LogInformation("No restaurants found for owner {OwnerId}", ownerId);
                return new List<ComplaintResponseDto>();
            }

            // Dobij sve žalbe za sve restorane vlasnika
            var complaints = await _complaintRepository.GetComplaintsByRestaurantIdsAsync(restaurantIds);
            _logger.LogInformation("Retrieved {Count} complaints for owner {OwnerId} with {RestaurantCount} restaurants", complaints.Count, ownerId, restaurantIds.Count);
            return complaints;
        }

        public async Task<ComplaintResponseDto> GetComplaintByOrderIdAsync(int orderId, int userId)
        {
            var complaint = await _complaintRepository.GetComplaintByOrderIdAndUserIdAsync(orderId, userId);
            if (complaint == null)
            {
                _logger.LogInformation("No complaint found for order {OrderId} by user {UserId}", orderId, userId);
            }
            return complaint;
        }
    }
}

