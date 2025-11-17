using Gozba_na_klik.DTOs.Invoice;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "RegisteredPolicy")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        // GET: api/invoices/order/{orderId}
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceByOrderId(int orderId)
        {
            var userId = int.Parse(User.FindFirstValue("userid") ?? throw new UnauthorizedAccessException());
            var invoice = await _invoiceService.GetInvoiceByOrderIdAsync(orderId, userId);
            return Ok(invoice);
        }

        // GET: api/invoices/{invoiceId}
        [HttpGet("{invoiceId}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceById(string invoiceId)
        {
            var userId = int.Parse(User.FindFirstValue("userid") ?? throw new UnauthorizedAccessException());
            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId, userId);
            return Ok(invoice);
        }

        // POST: api/invoices/regenerate/{orderId}
        [HttpPost("regenerate/{orderId}")]
        [Authorize(Policy = "OwnerOrAdminPolicy")]
        public async Task<ActionResult<InvoiceDto>> RegenerateInvoice(int orderId)
        {
            var userId = int.Parse(User.FindFirstValue("userid") ?? throw new UnauthorizedAccessException());
            var invoice = await _invoiceService.RegenerateInvoiceAsync(orderId, userId);
            return Ok(invoice);
        }

        // GET: api/invoices/test/generate-id
        [HttpGet("test/generate-id")]
        [Authorize(Policy = "AdminPolicy")]
        public ActionResult<object> GenerateTestInvoiceId()
        {
            var invoiceId = _invoiceService.GenerateInvoiceId();
            _logger.LogInformation("Generated test invoice ID: {InvoiceId}", invoiceId);
            return Ok(new { invoiceId });
        }

        // GET: api/invoices/order/{orderId}/pdf
        [HttpGet("order/{orderId}/pdf")]
        public async Task<IActionResult> GetInvoicePdf(int orderId)
        {
            var userId = int.Parse(User.FindFirstValue("userid") ?? throw new UnauthorizedAccessException());
            var pdfBytes = await _invoiceService.GenerateInvoicePdfBytesAsync(orderId, userId);
            var fileName = $"invoice-order-{orderId}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }

        // GET: api/invoices/{invoiceId}/pdf
        [HttpGet("{invoiceId}/pdf")]
        public async Task<IActionResult> GetInvoicePdfById(string invoiceId)
        {
            var userId = int.Parse(User.FindFirstValue("userid") ?? throw new UnauthorizedAccessException());
            var pdfBytes = await _invoiceService.GenerateInvoicePdfBytesByIdAsync(invoiceId, userId);
            var fileName = $"invoice-{invoiceId}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}