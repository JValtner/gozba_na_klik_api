using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Models.Customers;
using Gozba_na_klik.Services.AddressServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Gozba_na_klik.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _svc;

        public AddressesController(IAddressService svc)
        {
            _svc = svc;
        }

        private int RequireUserIdOrThrow()
        {
            StringValues raw;
            if (!Request.Headers.TryGetValue("X-User-Id", out raw))
            {
                throw new UnauthorizedAccessException("Missing X-User-Id");
            }

            int uidParsed;
            if (!int.TryParse(raw.ToString(), out uidParsed))
            {
                throw new UnauthorizedAccessException("Invalid X-User-Id");
            }

            return uidParsed;
        }

        private IActionResult MapExceptionToResult(Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException uae =>
                    string.Equals(uae.Message, "Missing X-User-Id", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(uae.Message, "Invalid X-User-Id", StringComparison.OrdinalIgnoreCase)
                        ? Unauthorized(uae.Message)                               // 401 – hiányzó/hibás header
                        : StatusCode(StatusCodes.Status403Forbidden),             // 403 – jogosultság hiánya (auth-scheme nélkül)
                KeyNotFoundException => NotFound(),                               // 404 – erőforrás nem található
                ArgumentException ae => BadRequest(ae.Message),                   // 400 – validációs hiba
                InvalidOperationException ioe => BadRequest(ioe.Message),         // 400 – üzleti hiba
                _ => Problem(statusCode: 500, detail: ex.Message)                 // 500 – váratlan hiba
            };
        }

        [HttpGet("my")]
        [ProducesResponseType(typeof(List<AddressListItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMy()
        {
            try
            {
                int uid = RequireUserIdOrThrow();
                List<Address> items = await _svc.GetMyAsync(uid);

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
            catch (Exception ex)
            {
                return MapExceptionToResult(ex);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(AddressListItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] AddressCreateDto dto)
        {
            try
            {
                int uid = RequireUserIdOrThrow();
                Address created = await _svc.CreateAsync(uid, dto);

                AddressListItemDto outDto = new AddressListItemDto
                {
                    Id = created.Id,
                    Label = created.Label,
                    Street = created.Street,
                    City = created.City,
                    PostalCode = created.PostalCode,
                    IsDefault = created.IsDefault
                };

                return CreatedAtAction(nameof(GetMy), new { }, outDto);
            }
            catch (Exception ex)
            {
                return MapExceptionToResult(ex);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] AddressUpdateDto dto)
        {
            try
            {
                int uid = RequireUserIdOrThrow();
                await _svc.UpdateAsync(uid, id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return MapExceptionToResult(ex);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int uid = RequireUserIdOrThrow();
                await _svc.DeleteAsync(uid, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return MapExceptionToResult(ex);
            }
        }

        [HttpPut("{id:int}/default")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetDefault(int id)
        {
            try
            {
                int uid = RequireUserIdOrThrow();
                await _svc.SetDefaultAsync(uid, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return MapExceptionToResult(ex);
            }
        }
    }
}
