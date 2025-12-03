using System.Linq;
using Gozba_na_klik.DTOs;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Enums;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Gozba_na_klik.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IReviewService _reviewService;

        public RestaurantsController(IRestaurantService restaurantService, IReviewService reviewService)
        {
            _restaurantService = restaurantService;
            _reviewService = reviewService;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _restaurantService.GetAllRestaurantsAsync();
            return Ok(result);
        }

        [Authorize(Policy = "PublicPolicy")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneAsync(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantDtoByIdAsync(id);
            return Ok(restaurant);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id:int}/admin")]
        public async Task<IActionResult> GetOneByAdminAsync(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdByAdminAsync(id);
            return Ok(restaurant);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        public async Task<IActionResult> PostByAdminAsync([FromBody] RequestCreateRestaurantByAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _restaurantService.CreateRestaurantByAdminAsync(dto);
            return Ok(result);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("{id}/admin-edit")]
        public async Task<IActionResult> UpdateByAdminAsync(int id, [FromBody] RequestUpdateRestaurantByAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _restaurantService.UpdateRestaurantByAdminAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Policy = "OwnerOrAdminPolicy")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyRestaurantsAsync()
        {
            var userId = User.GetUserId();
            var restaurants = await _restaurantService.GetRestaurantsByOwnerAsync(userId);
            var list = restaurants.Select(r => new RestaurantListItemDto
            {
                Id = r.Id,
                Name = r.Name,
                PhotoUrl = r.PhotoUrl,
                OwnerId = r.OwnerId,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });
            return Ok(list);
        }
        [Authorize(Policy = "PublicPolicy")]
        [HttpGet("filterSortPage")]
        public async Task<IActionResult> GetFilteredSortedPagedAsync(
        [FromQuery] RestaurantFilter filter,
        [FromQuery] int sortType = (int)RestaurantSortType.A_Z,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            var result = await _restaurantService.GetAllFilteredSortedPagedAsync(filter, sortType, page, pageSize);
            return Ok(result);
        }

        [Authorize(Policy = "OwnerOrAdminPolicy")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] RestaurantUpdateDto dto)
        {
            var userId = User.GetUserId();

            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                string folderPath = Path.Combine("assets", "restaurantImg");
                Directory.CreateDirectory(folderPath);
                string fileName = $"{Guid.NewGuid()}_{dto.Photo.FileName}";
                string filePath = Path.Combine(folderPath, fileName);
                using FileStream stream = new FileStream(filePath, FileMode.Create);
                await dto.Photo.CopyToAsync(stream);
                dto.PhotoUrl = "/assets/restaurantImg/" + fileName;
            }

            var updated = await _restaurantService.UpdateRestaurantByOwnerAsync(id, dto, userId);
            return Ok(new RestaurantListItemDto
            {
                Id = updated.Id,
                Name = updated.Name,
                PhotoUrl = updated.PhotoUrl,
                OwnerId = updated.OwnerId,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            });
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _restaurantService.GetRestaurantByIdOrThrowAsync(id);
            await _restaurantService.DeleteRestaurantAsync(id);
            return NoContent();
        }

        [Authorize(Policy = "OwnerOrAdminPolicy")]
        [HttpPut("{id}/workschedules")]
        public async Task<IActionResult> UpdateWorkSchedulesAsync(int id, [FromBody] List<WorkScheduleDto> scheduleDtos)
        {
            await _restaurantService.UpdateWorkSchedulesFromDtosAsync(id, scheduleDtos);
            return Ok(new { message = "Radno vreme uspešno ažurirano." });
        }

        [Authorize(Policy = "OwnerOrAdminPolicy")]
        [HttpPost("{id}/closeddates")]
        public async Task<IActionResult> AddClosedDateAsync(int id, [FromBody] ClosedDate date)
        {
            date.RestaurantId = id;
            await _restaurantService.AddClosedDateAsync(id, date);
            return Ok(new { message = "Neradni datum uspešno dodat." });
        }

        [Authorize(Policy = "OwnerOrAdminPolicy")]
        [HttpDelete("{id}/closeddates/{dateId}")]
        public async Task<IActionResult> RemoveClosedDateAsync(int id, int dateId)
        {
            await _restaurantService.RemoveClosedDateAsync(id, dateId);
            return NoContent();
        }

        [Authorize(Policy = "PublicPolicy")]
        [HttpGet("sortTypes")]
        public async Task<IActionResult> GetSortTypes()
        {
            var sortTypes = await _restaurantService.GetSortTypesAsync();
            return Ok(sortTypes);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("irresponsible")]
        public async Task<IActionResult> GetIrresponsibleRestaurantsAsync()
        {
            var result = await _restaurantService.GetIrresponsibleRestaurantsAsync();
            return Ok(result);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("{id}/suspend")]
        public async Task<IActionResult> SuspendRestaurantAsync(int id, [FromBody] SuspendRestaurantDto dto)
        {
            var adminId = User.GetUserId();
            var result = await _restaurantService.SuspendRestaurantAsync(id, dto.Reason, adminId);
            return Ok(result);
        }

        [Authorize(Policy = "OwnerOrAdminPolicy")]
        [HttpGet("{id}/suspension")]
        public async Task<IActionResult> GetRestaurantSuspensionAsync(int id)
        {
            var suspension = await _restaurantService.GetRestaurantSuspensionAsync(id);
            return Ok(suspension);
        }

        [Authorize(Policy = "OwnerOrAdminPolicy")]
        [HttpPost("{id}/suspension/appeal")]
        public async Task<IActionResult> AppealSuspensionAsync(int id, [FromBody] AppealSuspensionDto dto)
        {
            var ownerId = User.GetUserId();
            var result = await _restaurantService.AppealSuspensionAsync(id, dto.AppealText, ownerId);
            return Ok(result);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("suspension-appeals")]
        public async Task<IActionResult> GetAppealedSuspensionsAsync()
        {
            var appeals = await _restaurantService.GetAppealedSuspensionsAsync();
            return Ok(appeals);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch("{id}/suspension/decision")]
        public async Task<IActionResult> ProcessAppealDecisionAsync(int id, [FromBody] AppealDecisionDto dto)
        {
            var adminId = User.GetUserId();
            await _restaurantService.ProcessAppealDecisionAsync(id, dto.Accept, adminId);
            return Ok(new { message = dto.Accept ? "Žalba je prihvaćena i suspenzija je uklonjena." : "Žalba je odbijena." });
        }

        // GET: api/restaurants/{id}/reviews
        [Authorize(Policy = "PublicPolicy")]
        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetRestaurantReviews(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var reviews = await _reviewService.GetRestaurantReviewsAsync(id, page, pageSize);
            return Ok(reviews);
        }
    }
}