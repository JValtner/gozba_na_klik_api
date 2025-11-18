using Gozba_na_klik.DTOs.Invoice;

namespace Gozba_na_klik.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateInvoicePdfAsync(InvoiceDto invoice);
        bool CanGeneratePdf(InvoiceDto invoice);
    }
}
