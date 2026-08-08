using MedicalSupply.Domain.Enums;

namespace MedicalSupply.Api.Contracts.SupplyRequests
{
    public class RejectRequestDto
    {
        public ApprovalType ApprovalType { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
