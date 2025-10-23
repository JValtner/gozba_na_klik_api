using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services.AddressServices;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;

        public AddressesController(IAddressService addressService, IMapper mapper)
        {
            _addressService = addressService;
            _mapper = mapper;
        }

        // GET api/addresses/my
        [HttpGet("my")]
        public async Task<ActionResult<List<AddressListItemDto>>> GetMy(
            [FromHeader(Name = "X-User-Id")] int userId)
        {
            List<Address> items = await _addressService.GetMyAsync(userId);
            List<AddressListItemDto> dto = _mapper.Map<List<AddressListItemDto>>(items);

            return Ok(dto);
        }

        // POST api/addresses
        [HttpPost]
        public async Task<ActionResult<AddressListItemDto>> Create(
            [FromHeader(Name = "X-User-Id")] int userId,
            [FromBody] AddressCreateDto dtoIn)
        {
            Address created = await _addressService.CreateAsync(userId, dtoIn);

            AddressListItemDto dtoOut = _mapper.Map<AddressListItemDto>(created);

            return CreatedAtAction(nameof(GetMy), null, dtoOut);
        }

        // PUT api/addresses/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            [FromHeader(Name = "X-User-Id")] int userId,
            int id,
            [FromBody] AddressUpdateDto dto)
        {
            await _addressService.UpdateAsync(userId, id, dto);
            return NoContent();
        }

        // DELETE api/addresses/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromHeader(Name = "X-User-Id")] int userId,
            int id)
        {
            await _addressService.DeleteAsync(userId, id);
            return NoContent();
        }

        // PUT api/addresses/{id}/default
        [HttpPut("{id:int}/default")]
        public async Task<IActionResult> SetDefault(
            [FromHeader(Name = "X-User-Id")] int userId,
            int id)
        {
            await _addressService.SetDefaultAsync(userId, id);
            return NoContent();
        }
    }
}
