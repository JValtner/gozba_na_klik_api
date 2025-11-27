using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;

namespace Gozba_na_klik.Services.Snapshots
{
    public class MonthlyReportBuilder
    {
        private readonly IReportingRepository _reportingRepository;

        public MonthlyReportBuilder(IReportingRepository reportingRepository)
        {
            _reportingRepository = reportingRepository;
        }

        public async Task<MonthlyReportDTO> BuildAsync(int restaurantId, DateTime startUtc, DateTime endUtc)
        {
            // Fetch raw orders and order items
            var rawOrders = await _reportingRepository.GetOrdersForPeriod(restaurantId, startUtc, endUtc);
            var allOrderItems = rawOrders.SelectMany(o => o.Items).ToList();

            // Top 5 revenue orders
            var topRevenueOrders = rawOrders
                .OrderByDescending(o => o.TotalPrice)
                .Take(5)
                .Select(o => new OrderSummary
                {
                    OrderId = o.Id,
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate
                }).ToList();

            // Top & bottom meals
            var mealGroups = allOrderItems
                .GroupBy(i => new { i.MealId, i.Meal.Name })
                .Select(g => new PopularMealDTO
                {
                    MealId = g.Key.MealId,
                    MealName = g.Key.Name,
                    UnitsSold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.TotalPrice)
                }).ToList();

            var top5Meals = mealGroups.OrderByDescending(m => m.UnitsSold).Take(5).ToList();
            var bottom5Meals = mealGroups.OrderBy(m => m.UnitsSold).Take(5).ToList();
            var mostPopularUnits = top5Meals.Any() ? top5Meals.First().UnitsSold : 0;

            // Profit report
            var profitReport = new RestaurantProfitPeriodReportResponseDTO
            {
                DailyReports = rawOrders
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new RestaurantProfitDailyReportResponseDTO
                    {
                        RestaurantId = restaurantId,
                        Date = g.Key,
                        TotalDailyOrders = g.Count(),
                        DailyRevenue = g.Sum(o => o.TotalPrice)
                    }).ToList(),
                TotalPeriodOrders = rawOrders.Count,
                TotalRevenue = rawOrders.Sum(o => o.TotalPrice),
                AverageDailyProfit = rawOrders.Any() ? rawOrders.Average(o => o.TotalPrice) : 0
            };

            // Meal sales report
            var mealSalesReport = new MealSalesPeriodReportResponseDTO
            {
                DailyReports = allOrderItems
                    .GroupBy(i => i.Order.OrderDate.Date)
                    .Select(g => new MealSalesDailyReportResponseDTO
                    {
                        MealId = g.First().MealId,
                        MealName = g.First().Meal.Name,
                        Date = g.Key,
                        TotalDailyUnitsSold = g.Sum(x => x.Quantity),
                        DailyRevenue = g.Sum(x => x.TotalPrice)
                    }).ToList(),
                TotalUnitsSold = allOrderItems.Sum(i => i.Quantity),
                TotalRevenue = allOrderItems.Sum(i => i.TotalPrice),
                AverageDailyUnitsSold = allOrderItems.Any() ? (decimal)allOrderItems.Average(i => i.Quantity) : 0
            };

            // Orders report
            var ordersReport = new OrdersReportPeriodResponseDTO
            {
                TotalOrders = rawOrders.Count,
                TotalAcceptedOrders = rawOrders.Count(o => o.Status == OrderStatus.PRIHVAĆENA),
                TotalCancelledOrders = rawOrders.Count(o => o.Status == OrderStatus.OTKAZANA),
                TotalCompletedOrders = rawOrders.Count(o => o.Status == OrderStatus.ISPORUČENA),
                TotalPendingOrders = rawOrders.Count(o => o.Status == OrderStatus.NA_CEKANJU),
                TotalInDeliveryOrders = rawOrders.Count(o => o.Status == OrderStatus.U_DOSTAVI),
                TotalReadyOrders = rawOrders.Count(o => o.Status == OrderStatus.SPREMNA),
                TotalDeliveredOrders = rawOrders.Count(o => o.Status == OrderStatus.ISPORUČENA)
            };

            // Build final DTO
            return new MonthlyReportDTO
            {
                RestaurantId = restaurantId,
                Month = startUtc.Month,
                Year = startUtc.Year,
                TotalOrders = rawOrders.Count,
                TotalRevenue = rawOrders.Sum(o => o.TotalPrice),
                AverageOrderValue = rawOrders.Any() ? rawOrders.Average(o => o.TotalPrice) : 0,
                Top5RevenueOrders = new ListWithCountDTO<OrderSummary> { Items = topRevenueOrders, TotalCount = topRevenueOrders.Count },
                Top5PopularMeals = new ListWithCountDTO<PopularMealDTO> { Items = top5Meals, TotalCount = top5Meals.Sum(m => m.UnitsSold) },
                Bottom5PopularMeals = new ListWithCountDTO<PopularMealDTO> { Items = bottom5Meals, TotalCount = bottom5Meals.Sum(m => m.UnitsSold) },
                MostPopularMealUnitsSold = mostPopularUnits,
                ProfitReport = profitReport,
                MealSalesReport = mealSalesReport,
                OrdersReport = ordersReport
            };
        }
    }
}
