using Gozba_na_klik.DTOs.Invoice;

namespace Gozba_na_klik.Models
{
    public interface IInvoiceRepository
    {
        Task<InvoiceDto> SaveInvoiceAsync(InvoiceDto invoice);

        Task<InvoiceDto?> GetInvoiceByOrderIdAsync(int orderId);

        Task<InvoiceDto?> GetInvoiceByIdAsync(string invoiceId);

        Task<bool> InvoiceExistsForOrderAsync(int orderId);

        Task<bool> DeleteInvoiceAsync(string invoiceId);
    }
}