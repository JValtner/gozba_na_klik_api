using Gozba_na_klik.DTOs.Complaints;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Utils;
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
            var order = await _orderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
            {
                throw new NotFoundException($"Porudžbina sa ID {dto.OrderId} nije pronađena.");
            }

            if (order.UserId != userId)
            {
                throw new ForbiddenException("Nemate pravo da podnesete žalbu za ovu porudžbinu.");
            }

            var completedStatuses = new[] { "ZAVRŠENO", "ISPORUČENA" };
            if (!completedStatuses.Contains(order.Status))
            {
                throw new BadRequestException("Žalba se može podneti samo za završene porudžbine.");
            }
            var complaintExists = await _complaintRepository.ComplaintExistsForOrderAsync(dto.OrderId);
            if (complaintExists)
            {
                throw new BadRequestException("Već ste podneli žalbu za ovu porudžbinu.");
            }

            var restaurantId = order.RestaurantId;

            var complaint = await _complaintRepository.InsertComplaintAsync(dto, userId, restaurantId);

            _logger.LogInformation("Complaint created for order {OrderId} by user {UserId}", dto.OrderId, userId);

            return complaint;
        }

        public async Task<bool> HasComplaintForOrderAsync(int orderId, int userId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found for complaint check by user {UserId}", orderId, userId);
                    return false;
                }

                if (order.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to check complaint for order {OrderId} belonging to user {OrderUserId}", 
                        userId, orderId, order.UserId);
                    throw new ForbiddenException("Nemate pravo pristupa.");
                }

                var hasComplaint = await _complaintRepository.HasComplaintForOrderAsync(orderId, userId);
                _logger.LogInformation("Complaint check for order {OrderId} by user {UserId}: {HasComplaint}", orderId, userId, hasComplaint);
                return hasComplaint;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking complaint existence for order {OrderId} by user {UserId}", orderId, userId);
                return false;
            }
        }

        public async Task<ComplaintResponseDto> GetComplaintByOrderIdAsync(int orderId, int userId)
        {
            var complaint = await _complaintRepository.GetComplaintByOrderIdAndUserIdAsync(orderId, userId);
            if (complaint == null)
            {
                _logger.LogInformation("No complaint found for order {OrderId} by user {UserId}", orderId, userId);
                throw new NotFoundException("Žalba za ovu porudžbinu nije pronađena.");
            }
            return complaint;
        }

        public async Task<PaginatedList<ComplaintResponseDto>> GetAllComplaintsLast30DaysAsync(int page, int pageSize)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            _logger.LogInformation("Getting complaints from last 30 days for admin, page {Page}, pageSize {PageSize}", page, pageSize);

            return await _complaintRepository.GetAllComplaintsLast30DaysAsync(page, pageSize);
        }

        public async Task<ComplaintResponseDto> GetComplaintByIdAsync(string complaintId)
        {
            var complaint = await _complaintRepository.GetComplaintByIdAsync(complaintId);
            if (complaint == null)
            {
                _logger.LogInformation("Complaint with ID {ComplaintId} not found", complaintId);
                throw new NotFoundException("Žalba sa datim ID-jem nije pronađena.");
            }
            return complaint;
        }
    }
}

