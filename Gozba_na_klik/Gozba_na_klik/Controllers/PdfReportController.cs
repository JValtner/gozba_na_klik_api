using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Services.Pdf;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfReportController : ControllerBase
    {
        private readonly IPdfReportService _service;

        public PdfReportController(IPdfReportService service)
        {
            _service = service;
        }

        // --------------------------------------------------------------------
        // A) LIST STORED SNAPSHOTS
        // --------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> ListSnapshots(
            [FromQuery] int restaurantId,
            [FromQuery] int? year,
            [FromQuery] int? month)
        {
            var list = await _service.ListSnapshotsAsync(restaurantId, year, month);

            return Ok(list.Select(x => new
            {
                id = x.Id,
                restaurantId = x.RestaurantId,
                restaurantName = x.RestaurantName,
                year = x.Year,
                month = x.Month,
                createdUtc = x.CreatedUtc,
                totalOrders = x.TotalOrders,
                totalRevenue = x.TotalRevenue
            }));
        }

        // --------------------------------------------------------------------
        // B) MANUAL TRIGGER: GENERATE PREVIOUS MONTH SNAPSHOT & STORE IN MONGO
        // --------------------------------------------------------------------
        // Example:
        // POST api/PdfReport/autogenerate?restaurantId=5
        [HttpPost("autogenerate")]
        public async Task<IActionResult> AutoGenerate([FromQuery] int restaurantId)
        {
            var id = await _service.GenerateAndStorePreviousMonthSnapshotAsync(
                restaurantId, DateTime.UtcNow);

            return Ok(new { id });
        }

        // --------------------------------------------------------------------
        // C) DOWNLOAD SNAPSHOT PDF (FROM MONGO)
        // --------------------------------------------------------------------
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadSnapshotPdf([FromRoute] string id)
        {
            var bytes = await _service.GeneratePdfFromSnapshotAsync(id);
            return File(bytes, "application/pdf", $"mesecni-izvestaj-{id}.pdf");
        }

        // --------------------------------------------------------------------
        // D) ON-DEMAND MONTH PDF (NOT STORED)
        // --------------------------------------------------------------------
        // Cleaner version: frontend sends only restaurantId + year + month
        // {
        //    "restaurantId": 5,
        //    "year": 2025,
        //    "month": 11
        // }
        [HttpPost("on-demand/pdf")]
        public async Task<IActionResult> DownloadOnDemandPdf(
            [FromBody] OnDemandMonthlyReportRequest request)
        {
            var bytes = await _service.GenerateOnDemandMonthlyPdfAsync(request);

            var filename = $"mesecni-izvestaj-on-demand-{request.RestaurantId}-{request.Year}-{request.Month}.pdf";

            return File(bytes, "application/pdf", filename);
        }
    }
}
