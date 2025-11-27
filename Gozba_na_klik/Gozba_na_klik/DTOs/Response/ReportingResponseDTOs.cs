using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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

    public class MonthlyReportDTO
    {
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        // Existing fields
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public ListWithCountDTO<OrderSummary> Top5RevenueOrders { get; set; }
        public ListWithCountDTO<PopularMealDTO> Top5PopularMeals { get; set; }
        public ListWithCountDTO<PopularMealDTO> Bottom5PopularMeals { get; set; }
        public int MostPopularMealUnitsSold { get; set; }

        // New fields for full monthly report
        public RestaurantProfitPeriodReportResponseDTO ProfitReport { get; set; }
        public MealSalesPeriodReportResponseDTO MealSalesReport { get; set; }
        public OrdersReportPeriodResponseDTO OrdersReport { get; set; }
    }



    public class ListWithCountDTO<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class OrderSummary
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }

    //Mongo DB dto
    public class PdfMonthlyReportDocument
    {
        [BsonId] // Marks this as the MongoDB _id
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("restaurantId")]
        public int RestaurantId { get; set; }

        [BsonElement("restaurantName")]
        public string RestaurantName { get; set; }

        [BsonElement("year")]
        public int Year { get; set; }

        [BsonElement("month")]
        public int Month { get; set; }

        [BsonElement("totalOrders")]
        public int TotalOrders { get; set; }

        [BsonElement("totalRevenue")]
        public decimal TotalRevenue { get; set; }

        [BsonElement("averageOrderValue")]
        public decimal AverageOrderValue { get; set; }

        [BsonElement("top5PopularMeals")]
        public List<PopularMealDTO> Top5PopularMeals { get; set; }

        [BsonElement("bottom5PopularMeals")]
        public List<PopularMealDTO> Bottom5PopularMeals { get; set; }

        [BsonElement("top5RevenueOrders")]
        public List<OrderSummary> Top5RevenueOrders { get; set; }

        [BsonElement("profitReport")]
        public RestaurantProfitPeriodReportResponseDTO ProfitReport { get; set; }

        [BsonElement("mealSalesReport")]
        public MealSalesPeriodReportResponseDTO MealSalesReport { get; set; }

        [BsonElement("ordersReport")]
        public OrdersReportPeriodResponseDTO OrdersReport { get; set; }

        [BsonElement("mostPopularMealUnitsSold")]
        public int MostPopularMealUnitsSold { get; set; }

        [BsonElement("createdUtc")]
        public DateTime CreatedUtc { get; set; }
    }

    
}

