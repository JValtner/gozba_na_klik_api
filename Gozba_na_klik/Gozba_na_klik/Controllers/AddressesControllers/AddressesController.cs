using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gozba_na_klik.DTOs.Addresses;
using Gozba_na_klik.Models.Customers;
using Gozba_na_klik.Services.AddressServices;
using Microsoft.AspNetCore.Mvc;

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

		private int RequireUserId()
		{
			if (!Request.Headers.TryGetValue("X-User-Id", out Microsoft.Extensions.Primitives.StringValues raw))
			{
				throw new System.UnauthorizedAccessException("Missing X-User-Id");
			}
			int uidParsed;
			if (!int.TryParse(raw.ToString(), out uidParsed))
			{
				throw new System.UnauthorizedAccessException("Invalid X-User-Id");
			}
			return uidParsed;
		}

		[HttpGet("my")]
		public async Task<ActionResult<List<AddressListItemDto>>> GetMy()
		{
			int uid = RequireUserId();
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

		[HttpPost]
		public async Task<ActionResult<AddressListItemDto>> Create([FromBody] AddressCreateDto dto)
		{
			int uid = RequireUserId();
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

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, [FromBody] AddressUpdateDto dto)
		{
			int uid = RequireUserId();
			await _svc.UpdateAsync(uid, id, dto);
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			int uid = RequireUserId();
			await _svc.DeleteAsync(uid, id);
			return NoContent();
		}

		[HttpPut("{id:int}/default")]
		public async Task<IActionResult> SetDefault(int id)
		{
			int uid = RequireUserId();
			await _svc.SetDefaultAsync(uid, id);
			return NoContent();
		}
	}
}
