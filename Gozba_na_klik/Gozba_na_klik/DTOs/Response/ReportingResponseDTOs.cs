using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
namespace Gozba_na_klik.DTOs.Request
{
    public class RestaurantProfitDailyReportResponseDTO
    {
        public int RestaurantId { get; set; }
        public DateTime Date { get; set; }           
        public int TotalDailyOrders { get; set; }
        public decimal DailyRevenue { get; set; }
    }

    public class RestaurantProfitPeriodReportResponseDTO
    {
        public List<RestaurantProfitDailyReportResponseDTO> DailyReports { get; set; }
        public int TotalPeriodOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageDailyProfit { get; set; }
    }
    public class MealSalesDailyReportResponseDTO
    {
        public int MealId { get; set; }
        public string MealName { get; set; }          
        public DateTime Date { get; set; }            
        public int TotalDailyUnitsSold { get; set; }
        public decimal DailyRevenue { get; set; }
    }

    public class MealSalesPeriodReportResponseDTO
    {
        public List<MealSalesDailyReportResponseDTO> DailyReports { get; set; }
        public int TotalUnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageDailyUnitsSold { get; set; }
    }
    public class OrdersReportDailyResponseDTO
    {
        public DateTime Date { get; set; }            
        public int TotalOrders { get; set; }
        public int TotalAcceptedOrders { get; set; }
        public int TotalCancelledOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalPendingOrders { get; set; }
        public int TotalInDeliveryOrders { get; set; }
        public int TotalReadyOrders { get; set; }
        public int TotalDeliveredOrders { get; set; }
    }

    public class OrdersReportPeriodResponseDTO
    {
        public int TotalOrders { get; set; }
        public int TotalAcceptedOrders { get; set; }
        public int TotalCancelledOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalPendingOrders { get; set; }
        public int TotalInDeliveryOrders { get; set; }
        public int TotalReadyOrders { get; set; }
        public int TotalDeliveredOrders { get; set; }
    }


    public class PopularMealDTO
    {
        public int MealId { get; set; }
        public string MealName { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class MontlyReportDTO
    {
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }

        public ListWithCountDTO<Order> Top5RevenueOrders { get; set; }

        // Replace raw OrderItem with a safer DTO
        public ListWithCountDTO<PopularMealDTO> Top5PopularMeals { get; set; }
        public ListWithCountDTO<PopularMealDTO> Bottom5PopularMeals { get; set; }

        public int MostPopularMealUnitsSold { get; set; }
    }


    public class ListWithCountDTO<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}

