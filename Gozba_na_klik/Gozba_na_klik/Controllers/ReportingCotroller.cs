using Microsoft.AspNetCore.Mvc;
using Gozba_na_klik.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Gozba_na_klik.Services.Reporting;

namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ReportingController : ControllerBase
    {
        private readonly IReportingService _reportingService;
        private readonly ILogger<AlergensController> _logger;

        public ReportingController(IReportingService reportingService, ILogger<AlergensController> logger)
        {
            _reportingService = reportingService;
            _logger = logger;
        }
        // GET: api/profit-report
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("profit-report")]
        public async Task<IActionResult> GetRestaurantProfitReport([FromQuery] RestaurantProfitReportRequestDTO requestData )
        {
            _logger.LogInformation("Fetching all profit elements");
            var responseData = await _reportingService.GetRestaurantProfitReport(requestData);
            return Ok(responseData);
        }

        // GET: api/meal-sales-report
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("meal-sales-report")]
        public async Task<IActionResult> GetMealSalesReport([FromQuery] MealSalesReportRequestDTO requestData)
        {
            _logger.LogInformation("Fetching all meal sales elements");
            var responseData = await _reportingService.GetMealSalesReport(requestData);
            return Ok(responseData);
        }

        // GET: api/orders-report
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("orders-report")]
        public async Task<IActionResult> GetOrdersReport([FromQuery] RestaurantOrdersReportRequestDTO requestData)
        {
            _logger.LogInformation("Fetching all orders elements");
            var responseData = await _reportingService.GetOrdersReport(requestData);
            return Ok(responseData);
        }

        // GET: api/full-monthly-report
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("full-monthly-report")]
        public async Task<IActionResult> GetFullMonthlyReport([FromQuery] int restaurantId)
        {
            _logger.LogInformation("Fetching monthly report");
            var responseData = await _reportingService.GetMonthlyReportAsync(restaurantId);
            return Ok(responseData);
        }
    }
}
