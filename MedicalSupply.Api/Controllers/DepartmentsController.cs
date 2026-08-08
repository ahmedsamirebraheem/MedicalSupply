using MedicalSupply.Application.DTOs.Departments;
using MedicalSupply.Application.Features.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSupply.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentService _service;

        public DepartmentsController(DepartmentService service) => _service = service;

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto dto, CancellationToken ct)
        {
            var id = await _service.CreateDepartmentAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var department = await _service.GetByIdAsync(id, ct);
            return Ok(department);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var departments = await _service.GetAllAsync(ct);
            return Ok(departments);
        }
    }
}