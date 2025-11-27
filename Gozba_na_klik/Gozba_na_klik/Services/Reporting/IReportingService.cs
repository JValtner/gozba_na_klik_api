
using Gozba_na_klik.DTOs.Request;

namespace Gozba_na_klik.Services.Reporting
{
    public interface IReportingService
    {
        Task<RestaurantProfitPeriodReportResponseDTO> GetRestaurantProfitReport(RestaurantProfitReportRequestDTO request);
        Task<MealSalesPeriodReportResponseDTO> GetMealSalesReport(MealSalesReportRequestDTO request);
        Task<OrdersReportPeriodResponseDTO> GetOrdersReport(RestaurantOrdersReportRequestDTO request);
        Task<MonthlyReportDTO> BuildMonthlyReportAsync(int restaurantId, DateTime startUtc, DateTime endUtc);
        Task<MonthlyReportDTO> GetMonthlyReportAsync(int restaurantId);
    }
}
