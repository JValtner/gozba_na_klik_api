
using Gozba_na_klik.DTOs.Request;

namespace Gozba_na_klik.Services.Reporting
{
    public interface IReportingService
    {
        Task<RestaurantProfitPeriodReportResponseDTO> GetRestaurantProfitReport(RestaurantProfitReportRequestDTO request);
        Task<MealSalesPeriodReportResponseDTO> GetMealSalesReport(MealSalesReportRequestDTO request);
        Task<OrdersReportPeriodResponseDTO> GetOrdersReport(RestaurantOrdersReportRequestDTO request);

        // Unified monthly builder (arbitrary range)
        Task<MonthlyReportDTO> BuildMonthlyReportAsync(int restaurantId, DateTime startUtc, DateTime endUtc);

        // Backwards-compatible helper that returns last-30-days DTO
        Task<MonthlyReportDTO> GetMonthlyReportAsync(int restaurantId);
    }
}
