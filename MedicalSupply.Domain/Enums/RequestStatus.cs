using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Enums
{
    public enum RequestStatus
    {
        Draft=1,
        Submitted,
        PendingManagerApproval,
        PendingPharmacyApproval,
        PendingFinanceApproval,
        Approved,
        Rejected,
        Cancelled,
        Fulfilled
    }
}
