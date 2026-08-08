using MedicalSupply.Application.Abstractions.Persistence;
using MedicalSupply.Application.Common;
using MedicalSupply.Application.DTOs.Items;
using MedicalSupply.Application.Exceptions;
using MedicalSupply.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalSupply.Application.Features.Items
{
    public class ItemService
    {
        private readonly IApplicationDbContext _context;

        public ItemService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateItemAsync(
            CreateItemDto dto, CancellationToken cancellationToken = default)
        {
            var codeExists = await _context.Items
                .AnyAsync(i => i.Code == dto.Code, cancellationToken);

            if (codeExists)
                throw new BusinessRuleException($"An item with code '{dto.Code}' already exists.");

            var item = new Item(
                dto.Code,
                dto.Name,
                dto.Category,
                dto.UnitPrice,
                dto.AvailableQuantity,
                dto.RequiresPharmacyApproval,
                dto.IsControlledMedication);

            _context.AddItem(item);
            await _context.SaveChangesAsync(cancellationToken);

            return item.Id;
        }

        public async Task UpdateItemAsync(
            int id, UpdateItemDto dto, CancellationToken cancellationToken = default)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

            if (item is null)
                throw new NotFoundException($"Item with id {id} was not found.");

            item.UpdateDetails(dto.Code, dto.Name, dto.Category, dto.UnitPrice, dto.RequiresPharmacyApproval, dto.IsControlledMedication);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ItemDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

            if (item is null)
                throw new NotFoundException($"Item with id {id} was not found.");

            return MapToDto(item);
        }

        public async Task<PagedResult<ItemDto>> SearchItemsAsync(
            string? search, string? category, int page, int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.Code.Contains(search) || i.Name.Contains(search));

            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<Domain.Enums.ItemCategory>(category, true, out var categoryEnum))
                query = query.Where(i => i.Category == categoryEnum);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(i => i.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Code = i.Code,
                    Name = i.Name,
                    Category = i.Category.ToString(),
                    UnitPrice = i.UnitPrice,
                    AvailableQuantity = i.AvailableQuantity,
                    ReservedQuantity = i.ReservedQuantity,
                    RequiresPharmacyApproval = i.RequiresPharmacyApproval,
                    IsControlledMedication = i.IsControlledMedication,
                    IsActive = i.IsActive
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<ItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private static ItemDto MapToDto(Item item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                Category = item.Category.ToString(),
                UnitPrice = item.UnitPrice,
                AvailableQuantity = item.AvailableQuantity,
                ReservedQuantity = item.ReservedQuantity,
                RequiresPharmacyApproval = item.RequiresPharmacyApproval,
                IsControlledMedication = item.IsControlledMedication,
                IsActive = item.IsActive
            };
        }
    }
}