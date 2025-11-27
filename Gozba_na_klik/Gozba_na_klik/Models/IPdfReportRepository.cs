using Gozba_na_klik.DTOs.Request;

namespace Gozba_na_klik.Models
{
    public interface IPdfReportRepository
    {
        Task<PdfMonthlyReportDocument> GetByIdAsync(string id);
        Task<PdfMonthlyReportDocument> GetByRestaurantMonthAsync(int restaurantId, int year, int month);
        Task<List<PdfMonthlyReportDocument>> ListAsync(int restaurantId, int? year = null, int? month = null);
        Task<string> InsertAsync(PdfMonthlyReportDocument doc);
        Task<bool> ExistsAsync(int restaurantId, int year, int month);
    }

}
