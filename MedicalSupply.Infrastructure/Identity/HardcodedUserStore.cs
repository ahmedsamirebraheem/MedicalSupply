using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Identity
{
    public static class HardcodedUserStore
    {
        public static readonly List<HardcodedUser> Users = new()
        {
            new HardcodedUser { Email = "requester@company.com", Password = "Pass123!", Role = "Requester" },
            new HardcodedUser { Email = "manager@company.com", Password = "Pass123!", Role = "DepartmentManager" },
            new HardcodedUser { Email = "pharmacist@company.com", Password = "Pass123!", Role = "Pharmacist" },
            new HardcodedUser { Email = "finance@company.com", Password = "Pass123!", Role = "FinanceOfficer" },
            new HardcodedUser { Email = "storekeeper@company.com", Password = "Pass123!", Role = "StoreKeeper" },
            new HardcodedUser { Email = "admin@company.com", Password = "Pass123!", Role = "Administrator" }
        };
    }
}
