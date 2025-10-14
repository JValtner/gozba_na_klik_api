﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services.AddressServices;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // GET api/addresses/my
        [HttpGet("my")]
        public async Task<ActionResult<List<AddressListItemDto>>> GetMy(
            [FromHeader(Name = "X-User-Id")] int userId)
        {
            List<Address> items = await _addressService.GetMyAsync(userId);

            List<AddressListItemDto> dto = items.Select(a => new AddressListItemDto
            {
                Id = a.Id,
                Label = a.Label,
                Street = a.Street,
                City = a.City,
                PostalCode = a.PostalCode,
                IsDefault = a.IsDefault
            }).ToList();

            return Ok(dto);
        }

        // POST api/addresses
        [HttpPost]
        public async Task<ActionResult<AddressListItemDto>> Create(
            [FromHeader(Name = "X-User-Id")] int userId,
            [FromBody] AddressCreateDto dtoIn)
        {
            Address created = await _addressService.CreateAsync(userId, dtoIn);

            AddressListItemDto dtoOut = new AddressListItemDto
            {
                Id = created.Id,
                Label = created.Label,
                Street = created.Street,
                City = created.City,
                PostalCode = created.PostalCode,
                IsDefault = created.IsDefault
            };

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
