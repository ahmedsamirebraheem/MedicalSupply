using MedicalSupply.Domain.Enums;

namespace MedicalSupply.Api.Contracts.SupplyRequests
{
    public class ApproveRequestDto
    {
        public ApprovalType ApprovalType { get; set; }
        public string? Comments { get; set; }
    }
}