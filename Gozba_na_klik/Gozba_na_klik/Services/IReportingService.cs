using Gozba_na_klik.DTOs.Request;

public interface IReportingService
{
    Task<RestaurantProfitPeriodReportResponseDTO> GetRestaurantProfitReport(RestaurantProfitReportRequestDTO request);
    Task<MealSalesPeriodReportResponseDTO> GetMealSalesReport(MealSalesReportRequestDTO request);
    Task<OrdersReportPeriodResponseDTO> GetOrdersReport(RestaurantOrdersReportRequestDTO request);
    Task<MontlyReportDTO> GetMonthlyReport(int restaurantId);
}