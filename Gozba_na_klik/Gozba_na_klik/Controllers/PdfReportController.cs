using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> ListSnapshots(
            [FromQuery] int restaurantId,
            [FromQuery] int? year,
            [FromQuery] int? month)
        {
            var list = await _service.ListSnapshotsAsync(restaurantId, year, month);

            return Ok(list);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("autogenerate")]
        public async Task<IActionResult> AutoGenerate([FromQuery] int restaurantId)
        {
            var id = await _service.GenerateAndStorePreviousMonthSnapshotAsync(
                restaurantId, DateTime.UtcNow);

            return Ok(new { id });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadSnapshotPdf([FromRoute] string id)
        {
            var bytes = await _service.GeneratePdfFromSnapshotAsync(id);
            return File(bytes, "application/pdf", $"mesecni-izvestaj-{id}.pdf");
        }

        [Authorize(Policy = "AdminPolicy")]
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
