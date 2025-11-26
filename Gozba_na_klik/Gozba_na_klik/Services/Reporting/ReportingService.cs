using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Gozba_na_klik.Services.Snapshots;
using Microsoft.Extensions.Logging;

namespace Gozba_na_klik.Services.Reporting
{
    public class ReportingService : IReportingService
    {
        private readonly IReportingRepository _reportingRepository;
        private readonly MonthlyReportBuilder _builder;
        private readonly ILogger<ReportingService> _logger;

        public ReportingService(IReportingRepository reportingRepository, MonthlyReportBuilder builder, ILogger<ReportingService> logger)
        {
            _reportingRepository = reportingRepository;
            _builder = builder;
            _logger = logger;
        }

        public async Task<RestaurantProfitPeriodReportResponseDTO> GetRestaurantProfitReport(RestaurantProfitReportRequestDTO request)
        {
            var orders = await _reportingRepository.GetOrdersForPeriod(request.RestaurantId, request.StartDate, request.EndDate);

            var dailyReports = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new RestaurantProfitDailyReportResponseDTO
                {
                    RestaurantId = request.RestaurantId,
                    Date = g.Key,
                    TotalDailyOrders = g.Count(),
                    DailyRevenue = g.Sum(o => o.TotalPrice)
                }).ToList();

            return new RestaurantProfitPeriodReportResponseDTO
            {
                DailyReports = dailyReports,
                TotalPeriodOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalPrice),
                AverageDailyProfit = dailyReports.Any() ? dailyReports.Average(r => r.DailyRevenue) : 0
            };
        }

        public async Task<MealSalesPeriodReportResponseDTO> GetMealSalesReport(MealSalesReportRequestDTO request)
        {
            var items = await _reportingRepository.GetMealSalesForPeriod(request.RestaurantId, request.MealId, request.StartDate, request.EndDate);

            var dailyReports = items
                .GroupBy(i => i.Order.OrderDate.Date)
                .Select(g => new MealSalesDailyReportResponseDTO
                {
                    MealId = request.MealId,
                    Date = g.Key,
                    TotalDailyUnitsSold = g.Sum(i => i.Quantity),
                    DailyRevenue = g.Sum(i => i.TotalPrice)
                }).ToList();

            return new MealSalesPeriodReportResponseDTO
            {
                DailyReports = dailyReports,
                TotalUnitsSold = items.Sum(i => i.Quantity),
                TotalRevenue = items.Sum(i => i.TotalPrice),
                AverageDailyUnitsSold = dailyReports.Any() ? (decimal)dailyReports.Average(r => r.TotalDailyUnitsSold) : 0
            };
        }

        public async Task<OrdersReportPeriodResponseDTO> GetOrdersReport(RestaurantOrdersReportRequestDTO request)
        {
            var orders = await _reportingRepository.GetOrdersForPeriod(request.RestaurantId, request.StartDate, request.EndDate);

            return new OrdersReportPeriodResponseDTO
            {
                TotalOrders = orders.Count,
                TotalAcceptedOrders = orders.Count(o => o.Status == OrderStatus.PRIHVAĆENA),
                TotalCancelledOrders = orders.Count(o => o.Status == OrderStatus.OTKAZANA),
                TotalCompletedOrders = orders.Count(o => o.Status == OrderStatus.ISPORUČENA),
                TotalPendingOrders = orders.Count(o => o.Status == OrderStatus.NA_CEKANJU),
                TotalInDeliveryOrders = orders.Count(o => o.Status == OrderStatus.U_DOSTAVI),
                TotalReadyOrders = orders.Count(o => o.Status == OrderStatus.SPREMNA),
                TotalDeliveredOrders = orders.Count(o => o.Status == OrderStatus.ISPORUČENA)
            };
        }

        // Unified monthly builder
        public Task<MonthlyReportDTO> BuildMonthlyReportAsync(int restaurantId, DateTime startUtc, DateTime endUtc)
        {
            // delegate to builder
            return _builder.BuildAsync(restaurantId, startUtc, endUtc);
        }

        // Backwards-compatible convenience method (last 30 days)
        public Task<MonthlyReportDTO> GetMonthlyReportAsync(int restaurantId)
        {
            var end = DateTime.UtcNow;
            var start = end.AddDays(-30);
            return BuildMonthlyReportAsync(restaurantId, start, end);
        }
    }
}
