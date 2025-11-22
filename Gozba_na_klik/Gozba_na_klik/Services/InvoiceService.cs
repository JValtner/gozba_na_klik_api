using System.Text.Json;
using AutoMapper;
using Gozba_na_klik.DTOs.Invoice;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace Gozba_na_klik.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly GozbaNaKlikDbContext _context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPdfService _pdfService;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(
            GozbaNaKlikDbContext context,
            IInvoiceRepository invoiceRepository,
            IPdfService pdfService,
            IMapper mapper,
            ILogger<InvoiceService> logger)
        {
            _context = context;
            _invoiceRepository = invoiceRepository;
            _pdfService = pdfService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<InvoiceDto> GenerateInvoiceAsync(int orderId)
        {
            _logger.LogInformation("Generating invoice for order {OrderId}", orderId);

            var fullOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Restaurant)
                .Include(o => o.Address)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Meal)
                        .ThenInclude(m => m.Addons)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (fullOrder == null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.");
            }

            if (fullOrder.Status != "ZAVRŠENO")
            {
                throw new BadRequestException("Invoice can only be generated for completed orders.");
            }

            var invoiceId = GenerateInvoiceId();
            var timestamp = DateTime.UtcNow;

            var invoiceDto = new InvoiceDto
            {
                InvoiceId = invoiceId,
                Timestamp = timestamp,
                OrderId = fullOrder.Id,
                OrderDate = fullOrder.OrderDate,
                Customer = new CustomerInfoDto
                {
                    Id = fullOrder.User.Id,
                    Username = fullOrder.User.UserName ?? string.Empty,
                    Email = fullOrder.User.Email ?? string.Empty
                },
                Restaurant = new RestaurantInfoDto
                {
                    Id = fullOrder.Restaurant.Id,
                    Name = fullOrder.Restaurant.Name,
                    Address = fullOrder.Restaurant.Address,
                    Phone = fullOrder.Restaurant.Phone
                },
                DeliveryAddress = fullOrder.Address != null ? new AddressInfoDto
                {
                    Id = fullOrder.Address.Id,
                    Street = fullOrder.Address.Street,
                    City = fullOrder.Address.City,
                    PostalCode = fullOrder.Address.PostalCode,
                    Entrance = fullOrder.Address.Entrance,
                    Floor = fullOrder.Address.Floor,
                    Apartment = fullOrder.Address.Apartment
                } : new AddressInfoDto(),
                CustomerNote = fullOrder.CustomerNote,
                HasAllergenWarning = fullOrder.HasAllergenWarning,
                Payment = new PaymentInfoDto
                {
                    Method = "Gotovina",
                    Status = "Plaćeno",
                    PaymentDate = timestamp
                }
            };

            foreach (var item in fullOrder.Items)
            {
                var invoiceItem = new InvoiceItemDto
                {
                    MealId = item.Meal.Id,
                    MealName = item.Meal.Name,
                    MealImagePath = item.Meal.ImagePath,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                };

                if (!string.IsNullOrEmpty(item.SelectedAddons))
                {
                    try
                    {
                        var addonNames = JsonSerializer.Deserialize<List<string>>(item.SelectedAddons);
                        if (addonNames != null && addonNames.Any())
                        {
                            var addons = await _context.MealAddons
                                .Where(a => a.MealId == item.Meal.Id && addonNames.Contains(a.Name))
                                .ToListAsync();

                            invoiceItem.SelectedAddons = addons.Select(a => new InvoiceAddonDto
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Price = a.Price,
                                Type = a.Type
                            }).ToList();
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning("Error parsing addons for item {ItemId}: {Error}", item.Id, ex.Message);
                    }
                }

                invoiceDto.Items.Add(invoiceItem);
            }

            invoiceDto.Summary = new InvoiceSummaryDto
            {
                SubtotalPrice = fullOrder.SubtotalPrice,
                DeliveryFee = fullOrder.DeliveryFee,
                TotalPrice = fullOrder.TotalPrice,
                TotalItems = fullOrder.Items.Sum(i => i.Quantity)
            };

            _logger.LogInformation("Invoice {InvoiceId} successfully generated for order {OrderId}", invoiceId, fullOrder.Id);

            return invoiceDto;
        }

        public async Task<InvoiceDto> SaveInvoiceAsync(InvoiceDto invoiceDto)
        {
            _logger.LogInformation("Saving invoice {InvoiceId} to NoSQL database", invoiceDto.InvoiceId);

            var savedInvoice = await _invoiceRepository.SaveInvoiceAsync(invoiceDto);

            _logger.LogInformation("Invoice {InvoiceId} successfully saved", invoiceDto.InvoiceId);

            return savedInvoice;
        }

        public async Task<InvoiceDto> GetInvoiceByOrderIdAsync(int orderId, int userId)
        {
            _logger.LogInformation("User {UserId} requesting invoice for order {OrderId}", userId, orderId);

            bool isAdmin = await IsUserAdminAsync(userId);

            if (!isAdmin)
            {
                await ValidateOrderOwnershipAsync(orderId, userId);
            }
            else
            {
                _logger.LogInformation("Admin {UserId} accessing order {OrderId} - ownership validation skipped", userId, orderId);
            }

            var invoice = await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice for completed order {OrderId} not found", orderId);
                throw new NotFoundException("Invoice for the specified order not found.");
            }

            return invoice;
        }

        public async Task<InvoiceDto> GetInvoiceByIdAsync(string invoiceId, int userId)
        {
            _logger.LogInformation("User {UserId} requesting invoice {InvoiceId}", userId, invoiceId);

            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new NotFoundException("Invoice with the specified ID not found.");
            }

            bool isAdmin = await IsUserAdminAsync(userId);

            if (!isAdmin && invoice.Customer.Id != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access invoice {InvoiceId} belonging to user {OwnerId}",
                    userId, invoiceId, invoice.Customer.Id);
                throw new ForbiddenException("You do not have access to this invoice.");
            }
            else if (isAdmin)
            {
                _logger.LogInformation("Admin {UserId} accessing invoice {InvoiceId} - ownership check bypassed", userId, invoiceId);
            }

            return invoice;
        }

        public async Task<InvoiceDto> RegenerateInvoiceAsync(int orderId, int userId)
        {
            _logger.LogInformation("User {UserId} requesting invoice regeneration for order {OrderId}", userId, orderId);

            bool isAdmin = await IsUserAdminAsync(userId);

            if (!isAdmin)
            {
                await ValidateOrderOwnershipAsync(orderId, userId);
            }
            else
            {
                _logger.LogInformation("Admin {UserId} regenerating invoice for order {OrderId} - ownership validation skipped", userId, orderId);
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new NotFoundException("Order not found.");
            }

            if (order.Status != "ZAVRŠENO")
            {
                throw new BadRequestException("Invoice can only be regenerated for completed orders.");
            }

            var existingInvoice = await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId);
            if (existingInvoice != null)
            {
                await _invoiceRepository.DeleteInvoiceAsync(existingInvoice.InvoiceId);
                _logger.LogInformation("Deleted existing invoice {InvoiceId} for order {OrderId}",
                    existingInvoice.InvoiceId, orderId);
            }

            var newInvoice = await GenerateInvoiceAsync(orderId);
            await SaveInvoiceAsync(newInvoice);

            _logger.LogInformation("Successfully regenerated invoice {InvoiceId} for order {OrderId}",
                newInvoice.InvoiceId, orderId);

            return newInvoice;
        }

        public async Task<InvoiceDto?> FindInvoiceByOrderIdAsync(int orderId)
        {
            _logger.LogInformation("Internal request for invoice by order ID {OrderId}", orderId);
            return await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId);
        }

        public async Task<InvoiceDto?> FindInvoiceByIdAsync(string invoiceId)
        {
            _logger.LogInformation("Internal request for invoice by ID {InvoiceId}", invoiceId);
            return await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
        }

        public string GenerateInvoiceId()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
            var guid = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

            return $"INV-{timestamp}-{guid}";
        }

        public async Task<bool> IsUserAdminAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return false;

                bool isAdmin = (user.UserName ?? "").Contains("admin", StringComparison.OrdinalIgnoreCase);

                _logger.LogInformation("Admin check for user {UserId} ({Username}): {IsAdmin}",
                userId, user.UserName, isAdmin);

                return isAdmin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking admin status for user {UserId}", userId);
                return false;
            }
        }

        private async Task ValidateOrderOwnershipAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new NotFoundException("Order not found.");
            }

            if (order.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access order {OrderId} belonging to user {OwnerId}",
                    userId, orderId, order.UserId);
                throw new ForbiddenException("You do not have access to this order.");
            }

            if (order.Status != "ZAVRŠENO")
            {
                throw new BadRequestException("Invoice is only available for completed orders.");
            }
        }

        public async Task<byte[]> GenerateInvoicePdfBytesAsync(int orderId, int userId)
        {
            var invoice = await GetInvoiceByOrderIdAsync(orderId, userId);

            if (!_pdfService.CanGeneratePdf(invoice))
            {
                throw new BadRequestException("Cannot generate PDF for this invoice - missing required data");
            }

            return await _pdfService.GenerateInvoicePdfAsync(invoice);
        }

        public async Task<byte[]> GenerateInvoicePdfBytesByIdAsync(string invoiceId, int userId)
        {
            var invoice = await GetInvoiceByIdAsync(invoiceId, userId);

            if (!_pdfService.CanGeneratePdf(invoice))
            {
                throw new BadRequestException("Cannot generate PDF for this invoice - missing required data");
            }

            return await _pdfService.GenerateInvoicePdfAsync(invoice);
        }
    }
}