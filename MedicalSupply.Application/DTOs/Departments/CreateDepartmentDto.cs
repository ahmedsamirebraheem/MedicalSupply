using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.Departments
{
    public class CreateDepartmentDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal MonthlyBudget { get; set; }
    }
}
