using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;
using Gozba_na_klik.Repositories;
using Microsoft.Extensions.Logging;

namespace Gozba_na_klik.Services
{
    public class ReportingService: IReportingService
    {
        private readonly IReportingRepository _reportingRepository;
        private readonly GozbaNaKlikDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;

        public ReportingService(IReportingRepository reportingRepository, GozbaNaKlikDbContext context, IMapper mapper, ILogger<RestaurantService> logger)
        {
            _reportingRepository = reportingRepository;
            _context = context;
            _mapper = mapper;
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
                .Select(g => new MealSalesReportResponseDTO
                {
                    MealId = request.MealId,
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

            var dailyReports = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new OrdersReportDailyResponseDTO
                {
                    OrderId = g.First().Id,
                    TotalOrders = g.Count(),
                    TotalAcceptedOrders = g.Count(o => o.Status == OrderStatus.PRIHVAĆENA),
                    TotalCancelledOrders = g.Count(o => o.Status == OrderStatus.OTKAZANA),
                    TotalCompletedOrders = g.Count(o => o.Status == OrderStatus.ISPORUČENA),
                    TotalPendingOrders = g.Count(o => o.Status == OrderStatus.NA_CEKANJU),
                    TotalInDeliveryOrders = g.Count(o => o.Status == OrderStatus.U_DOSTAVI),
                    TotalReadyOrders = g.Count(o => o.Status == OrderStatus.SPREMNA),
                    TotalDeliveredOrders = g.Count(o => o.Status == OrderStatus.ISPORUČENA)
                }).ToList();

            return new OrdersReportPeriodResponseDTO
            {
                DailyReports = dailyReports,
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
        public async Task<MontlyReportDTO> GetMonthlyReport(int restaurantId)
        {
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            var orders = await _reportingRepository.GetOrdersForPeriod(restaurantId, startDate, endDate);

            var report = new MontlyReportDTO
            {
                RestaurantId = restaurantId,
                Month = DateTime.UtcNow.Month,
                Year = DateTime.UtcNow.Year,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalPrice),
                AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalPrice) : 0,

                Top5RevenueOrders = new ListWithCountDTO<Order>
                {
                    Items = orders.OrderByDescending(o => o.TotalPrice).Take(5).ToList(),
                    TotalCount = orders.Count
                },

                Top5PopularMeals = new ListWithCountDTO<Meal>
                {
                    Items = orders.SelectMany(o => o.Items)
                                  .GroupBy(i => i.Meal)
                                  .OrderByDescending(g => g.Count())
                                  .Take(5)
                                  .Select(g => g.Key)
                                  .ToList(),
                    TotalCount = orders.SelectMany(o => o.Items).Count()
                },

                Bottom5PopularMeals = new ListWithCountDTO<Meal>
                {
                    Items = orders.SelectMany(o => o.Items)
                                  .GroupBy(i => i.Meal)
                                  .OrderBy(g => g.Count())
                                  .Take(5)
                                  .Select(g => g.Key)
                                  .ToList(),
                    TotalCount = orders.SelectMany(o => o.Items).Count()
                },

                MostPopularMealUnitsSold = orders.SelectMany(o => o.Items)
                                                 .GroupBy(i => i.MealId)
                                                 .OrderByDescending(g => g.Sum(i => i.Quantity))
                                                 .FirstOrDefault()?.Sum(i => i.Quantity) ?? 0
            };

            // Save to NoSQL (MongoDB)
            // await _mongoCollection.InsertOneAsync(report);

            return report;
        }
    }
}
