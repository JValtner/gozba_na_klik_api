using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Models;
using Gozba_na_klik.Models.Orders;

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
                    Date =g.Key,
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
                    Date =g.Key,
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
            var allItems = orders.SelectMany(o => o.Items);

            // Project grouped meals into PopularMealDTO
            var mealAggregates = allItems
                .GroupBy(i => i.MealId)
                .Select(g => new PopularMealDTO
                {
                    MealId = g.Key,
                    MealName = g.First().Meal?.Name ?? "Unknown",
                    UnitsSold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.TotalPrice)
                })
                .ToList();

            var report = new MontlyReportDTO
            {
                RestaurantId = restaurantId,
                Restaurant = orders.FirstOrDefault()?.Restaurant,
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

                Top5PopularMeals = new ListWithCountDTO<PopularMealDTO>
                {
                    Items = mealAggregates.OrderByDescending(m => m.UnitsSold).Take(5).ToList(),
                    TotalCount = mealAggregates.Sum(m => m.UnitsSold)
                },

                Bottom5PopularMeals = new ListWithCountDTO<PopularMealDTO>
                {
                    Items = mealAggregates.OrderBy(m => m.UnitsSold).Take(5).ToList(),
                    TotalCount = mealAggregates.Sum(m => m.UnitsSold)
                },

                MostPopularMealUnitsSold = mealAggregates
                    .OrderByDescending(m => m.UnitsSold)
                    .FirstOrDefault()?.UnitsSold ?? 0
            };

            return report;
        }
    }
}
