using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.DTOs.Departments
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public decimal MonthlyBudget { get; set; }
    }
}
