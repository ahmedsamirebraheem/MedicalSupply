using MedicalSupply.Application.DTOs.Items;
using MedicalSupply.Application.Features.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSupply.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly ItemService _service;

        public ItemsController(ItemService service) => _service = service;

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateItemDto dto, CancellationToken ct)
        {
            var id = await _service.CreateItemAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateItemDto dto, CancellationToken ct)
        {
            await _service.UpdateItemAsync(id, dto, ct);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var item = await _service.GetByIdAsync(id, ct);
            return Ok(item);
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? search, [FromQuery] string? category,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await _service.SearchItemsAsync(search, category, page, pageSize, ct);
            return Ok(result);
        }
    }
}