using Microsoft.AspNetCore.Mvc;
using Gozba_na_klik.Services.CurrencyService;

namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("convert")]
        public async Task<IActionResult> Convert([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount)
        {
            // All validations and error handling are done in the service
            var converted = await _currencyService.ConvertAsync(amount, from, to);
            return Ok(new { converted });
        }
    }
}
