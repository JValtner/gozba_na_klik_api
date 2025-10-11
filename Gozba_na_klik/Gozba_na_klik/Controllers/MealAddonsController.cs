﻿using AutoMapper;
using Gozba_na_klik.DTOs.Request;
using Gozba_na_klik.DTOs.Response;
using Gozba_na_klik.Models;
using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealAddonsController : ControllerBase
    {
        private readonly IMealAddonService _mealAddonService;
        private readonly IMapper _mapper;
        private readonly ILogger<MealAddonsController> _logger;

        public MealAddonsController(
            IMealAddonService mealAddonService,
            IMapper mapper,
            ILogger<MealAddonsController> logger)
        {
            _mealAddonService = mealAddonService;
            _mapper = mapper;
            _logger = logger;
        }

        // ---------- GET: api/mealaddons ----------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResponseAddonDTO>>> GetAllAsync()
        {
            var addons = await _mealAddonService.GetAllMealAddonsAsync();
            var response = _mapper.Map<IEnumerable<ResponseAddonDTO>>(addons);
            return Ok(response);
        }

        // ---------- GET: api/mealaddons/{id} ----------
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseAddonDTO>> GetByIdAsync(int id)
        {
            var addon = await _mealAddonService.GetMealAddonByIdAsync(id);
            if (addon == null)
                return NotFound();

            var response = _mapper.Map<ResponseAddonDTO>(addon);
            return Ok(response);
        }

        // ---------- POST: api/mealaddons ----------
        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<ResponseAddonDTO>> CreateAsync([FromBody] RequestAddonDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Pass DTO directly to service
            var created = await _mealAddonService.CreateMealAddonAsync(request);

            _logger.LogInformation("Addon '{Name}' created for MealId {MealId}", created.Name, created.MealId);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
        }


        // ---------- DELETE: api/mealaddons/{id} ----------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var exists = await _mealAddonService.MealAddonExistsAsync(id);
            if (!exists)
                return NotFound();

            await _mealAddonService.DeleteMealAddonAsync(id);
            _logger.LogInformation("Addon with ID {Id} deleted", id);
            return NoContent();
        }
    }
}
