using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Entities
{
    public class Department
    {
        public int Id { get; private set; }
        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public bool IsActive { get; private set; }
        public decimal MonthlyBudget { get; private set; }
        private Department()
        {
            
        }
        public Department(string code, string name, decimal monthlyBudget)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Department code is required.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Department name is required.");

            if (monthlyBudget < 0)
                throw new ArgumentException("Monthly budget cannot be negative.");

            Code = code;
            Name = name;
            MonthlyBudget = monthlyBudget;
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
