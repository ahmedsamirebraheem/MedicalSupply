using MedicalSupply.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.SupplyRequests
{
    public class SearchSupplyRequestsQuery
    {
        public int? DepartmentId { get; set; }
        public RequestStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
