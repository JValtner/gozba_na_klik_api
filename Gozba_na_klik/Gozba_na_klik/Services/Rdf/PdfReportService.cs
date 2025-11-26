using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services.Reporting;

namespace Gozba_na_klik.Services.Pdf
{
    public class PdfReportService : IPdfReportService
    {
        private readonly IPdfReportRepository _repo;
        private readonly IReportingService _reportingService;
        private readonly IRestaurantService _restaurantService;
        private readonly IPdfRenderer _pdf;
        private readonly ILogger<PdfReportService> _logger;

        public PdfReportService(IPdfReportRepository repo,
                                IReportingService reportingService,
                                IRestaurantService restaurantService,
                                IPdfRenderer pdf,
                                ILogger<PdfReportService> logger)
        {
            _repo = repo;
            _reportingService = reportingService;
            _restaurantService = restaurantService;
            _pdf = pdf;
            _logger = logger;
        }

        // Generate snapshot for previous month and store in Mongo
        public async Task<string> GenerateAndStorePreviousMonthSnapshotAsync(int restaurantId, DateTime nowUtc)
        {
            var previousMonthStart = new DateTime(nowUtc.Year, nowUtc.Month, 1).AddMonths(-1);
            var year = previousMonthStart.Year;
            var month = previousMonthStart.Month;

            if (await _repo.ExistsAsync(restaurantId, year, month))
            {
                _logger.LogInformation("Snapshot already exists for {restaurantId} {year}/{month}", restaurantId, year, month);
                return null;
            }

            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1).AddTicks(-1);

            // Build full monthly report with profit, meals, and orders
            var monthlyDto = await _reportingService.BuildMonthlyReportAsync(restaurantId, start, end);
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(restaurantId);

            var doc = MapMonthlyToSnapshot(monthlyDto, restaurant?.Name, year, month);
            return await _repo.InsertAsync(doc);
        }

        // Convert snapshot to PDF
        public async Task<byte[]> GeneratePdfFromSnapshotAsync(string snapshotId)
        {
            var doc = await _repo.GetByIdAsync(snapshotId);
            if (doc == null) throw new KeyNotFoundException("Snapshot not found");

            // Map back to MonthlyReportDTO including new reports
            var dto = new MonthlyReportDTO
            {
                RestaurantId = doc.RestaurantId,
                Restaurant = new Restaurant { Id = doc.RestaurantId, Name = doc.RestaurantName },
                Year = doc.Year,
                Month = doc.Month,
                TotalOrders = doc.TotalOrders,
                TotalRevenue = doc.TotalRevenue,
                AverageOrderValue = doc.AverageOrderValue,
                Top5PopularMeals = new ListWithCountDTO<PopularMealDTO>
                {
                    Items = doc.Top5PopularMeals,
                    TotalCount = doc.Top5PopularMeals.Sum(m => m.UnitsSold)
                },
                Bottom5PopularMeals = new ListWithCountDTO<PopularMealDTO>
                {
                    Items = doc.Bottom5PopularMeals,
                    TotalCount = doc.Bottom5PopularMeals.Sum(m => m.UnitsSold)
                },
                Top5RevenueOrders = new ListWithCountDTO<OrderSummary>
                {
                    Items = doc.Top5RevenueOrders,
                    TotalCount = doc.Top5RevenueOrders.Count
                },
                MostPopularMealUnitsSold = doc.MostPopularMealUnitsSold,

                // NEW
                ProfitReport = doc.ProfitReport,
                MealSalesReport = doc.MealSalesReport,
                OrdersReport = doc.OrdersReport
            };

            return _pdf.Render(dto);
        }

        public async Task<byte[]> GeneratePdfFromMonthlyReportAsync(MonthlyReportDTO monthlyReport)
        {
            return _pdf.Render(monthlyReport); // full report
        }

        public async Task<List<PdfMonthlyReportDocument>> ListSnapshotsAsync(int restaurantId, int? year = null, int? month = null)
        {
            var list = await _repo.ListAsync(restaurantId, year, month);
            return list.Select(x => new PdfMonthlyReportDocument
            {
                Id = x.Id,
                RestaurantId = x.RestaurantId,
                RestaurantName = x.RestaurantName,
                Year = x.Year,
                Month = x.Month,
                CreatedUtc = x.CreatedUtc,
                TotalOrders = x.TotalOrders,
                TotalRevenue = x.TotalRevenue,
                AverageOrderValue = x.AverageOrderValue,
                Top5PopularMeals = x.Top5PopularMeals,
                Bottom5PopularMeals = x.Bottom5PopularMeals,
                Top5RevenueOrders = x.Top5RevenueOrders,
                MostPopularMealUnitsSold = x.MostPopularMealUnitsSold,
                ProfitReport = x.ProfitReport,
                MealSalesReport = x.MealSalesReport,
                OrdersReport = x.OrdersReport
            }).ToList();
        }

        private PdfMonthlyReportDocument MapMonthlyToSnapshot(MonthlyReportDTO m, string restaurantName, int year, int month)
        {
            return new PdfMonthlyReportDocument
            {
                RestaurantId = m.RestaurantId,
                RestaurantName = restaurantName ?? m.Restaurant?.Name ?? "Unknown",
                Year = year,
                Month = month,
                TotalOrders = m.TotalOrders,
                TotalRevenue = m.TotalRevenue,
                AverageOrderValue = m.AverageOrderValue,
                Top5PopularMeals = m.Top5PopularMeals?.Items ?? new List<PopularMealDTO>(),
                Bottom5PopularMeals = m.Bottom5PopularMeals?.Items ?? new List<PopularMealDTO>(),
                Top5RevenueOrders = m.Top5RevenueOrders?.Items ?? new List<OrderSummary>(),
                MostPopularMealUnitsSold = m.MostPopularMealUnitsSold,
                ProfitReport = m.ProfitReport,
                MealSalesReport = m.MealSalesReport,
                OrdersReport = m.OrdersReport,
                CreatedUtc = DateTime.UtcNow
            };
        }

        public async Task<byte[]> GenerateOnDemandMonthlyPdfAsync(OnDemandMonthlyReportRequest request)
        {
            var start = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1).AddTicks(-1);

            var dto = await _reportingService.BuildMonthlyReportAsync(request.RestaurantId, start, end);
            return _pdf.Render(dto);
        }
    }

}
