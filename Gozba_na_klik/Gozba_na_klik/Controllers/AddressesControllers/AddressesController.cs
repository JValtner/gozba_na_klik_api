using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Services.AddressServices;
using Gozba_na_klik.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    [Authorize(Policy = "UserPolicy")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // GET api/addresses/my
        [HttpGet("my")]
        public async Task<ActionResult<List<AddressListItemDto>>> GetMy()
        {
            var userId = User.GetUserId();
            var items = await _addressService.GetMyAsync(userId);
            return Ok(items);
        }

        // POST api/addresses
        [HttpPost]
        public async Task<ActionResult<AddressListItemDto>> Create([FromBody] AddressCreateDto dtoIn)
        {
            var userId = User.GetUserId();
            var created = await _addressService.CreateAsync(userId, dtoIn);
            return CreatedAtAction(nameof(GetMy), null, created);
        }

        // PUT api/addresses/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AddressUpdateDto dto)
        {
            var userId = User.GetUserId();
            await _addressService.UpdateAsync(userId, id, dto);
            return NoContent();
        }

        // DELETE api/addresses/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.GetUserId();
            await _addressService.DeleteAsync(userId, id);
            return NoContent();
        }

        // PUT api/addresses/{id}/default
        [HttpPut("{id:int}/default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var userId = User.GetUserId();
            await _addressService.SetDefaultAsync(userId, id);
            return NoContent();
        }
    }
}
