using MedicalSupply.Api.Contracts.SupplyRequests;
using MedicalSupply.Application.Abstractions.Security;
using MedicalSupply.Application.DTOs.SupplyRequests;
using MedicalSupply.Application.Services.SupplyRequests;
using MedicalSupply.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalSupply.Api.Controllers
{
    [Route("api/supply-requests")]
    [ApiController]
    [Authorize]
    public class SupplyRequestsController : ControllerBase
    {
        private readonly SupplyRequestService _service;
        private readonly ICurrentUserService _currentUser;

        public SupplyRequestsController(SupplyRequestService service, ICurrentUserService currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [HttpPost]
        [Authorize(Roles = "Requester,Administrator")]
        public async Task<IActionResult> Create([FromBody] CreateSupplyRequestDto dto, CancellationToken ct)
        {
            var id = await _service.CreateDraftRequestAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _service.GetRequestDetailsAsync(id, ct);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchSupplyRequestsQuery query, CancellationToken ct)
        {
            var result = await _service.SearchSupplyRequestsAsync(query, ct);
            return Ok(result);
        }

        [HttpPost("{id}/submit")]
        [Authorize(Roles = "Requester,Administrator")]
        public async Task<IActionResult> Submit(int id, CancellationToken ct)
        {
            await _service.SubmitRequestAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "DepartmentManager,Pharmacist,FinanceOfficer,Administrator")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveRequestDto dto, CancellationToken ct)
        {
            await _service.ApproveRequestAsync(id, dto.ApprovalType, _currentUser.Email, dto.Comments, ct);
            return NoContent();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "DepartmentManager,Pharmacist,FinanceOfficer,Administrator")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectRequestDto dto, CancellationToken ct)
        {
            await _service.RejectRequestAsync(id, dto.ApprovalType, _currentUser.Email, dto.Reason, ct);
            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        {
            await _service.CancelRequestAsync(id, ct);
            return NoContent();
        }

        [HttpPost("{id}/fulfill")]
        [Authorize(Roles = "StoreKeeper,Administrator")]
        public async Task<IActionResult> Fulfill(int id, CancellationToken ct)
        {
            await _service.FulfillRequestAsync(id, ct);
            return NoContent();
        }
    }
}