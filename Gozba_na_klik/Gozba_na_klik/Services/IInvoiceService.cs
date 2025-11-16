using Gozba_na_klik.DTOs.Invoice;
using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GenerateInvoiceAsync(Order order);
        Task<InvoiceDto> SaveInvoiceAsync(InvoiceDto invoiceDto);
        Task<InvoiceDto> GetInvoiceByOrderIdAsync(int orderId, int userId);
        Task<InvoiceDto> GetInvoiceByIdAsync(string invoiceId, int userId);
        Task<InvoiceDto> RegenerateInvoiceAsync(int orderId, int userId);
        Task<InvoiceDto?> GetInvoiceByOrderIdAsync(int orderId);
        Task<InvoiceDto?> GetInvoiceByIdAsync(string invoiceId);
        Task<byte[]> GenerateInvoicePdfBytesAsync(int orderId, int userId);
        Task<byte[]> GenerateInvoicePdfBytesByIdAsync(string invoiceId, int userId);
        string GenerateInvoiceId();
        Task<bool> IsUserAdminAsync(int userId);
    }
}