using MedicalSupply.Application.Abstractions.Persistence;
using MedicalSupply.Application.DTOs.Departments;
using MedicalSupply.Application.Exceptions;
using MedicalSupply.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalSupply.Application.Features.Departments
{
    public class DepartmentService
    {
        private readonly IApplicationDbContext _context;

        public DepartmentService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateDepartmentAsync(
            CreateDepartmentDto dto, CancellationToken cancellationToken = default)
        {
            var codeExists = await _context.Departments
                .AnyAsync(d => d.Code == dto.Code, cancellationToken);

            if (codeExists)
                throw new BusinessRuleException($"A department with code '{dto.Code}' already exists.");

            var department = new Department(dto.Code, dto.Name, dto.MonthlyBudget);

            _context.AddDepartment(department);
            await _context.SaveChangesAsync(cancellationToken);

            return department.Id;
        }

        public async Task<DepartmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (department is null)
                throw new NotFoundException($"Department with id {id} was not found.");

            return MapToDto(department);
        }

        public async Task<List<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Code = d.Code,
                    Name = d.Name,
                    IsActive = d.IsActive,
                    MonthlyBudget = d.MonthlyBudget
                })
                .ToListAsync(cancellationToken);
        }

        private static DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                Code = department.Code,
                Name = department.Name,
                IsActive = department.IsActive,
                MonthlyBudget = department.MonthlyBudget
            };
        }
    }
}