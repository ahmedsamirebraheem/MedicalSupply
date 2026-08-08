using MedicalSupply.Application.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Persistence
{
    public class RequestNumberGenerator : IRequestNumberGenerator
    {
        private readonly MedicalSupplyDbContext _context;

        public RequestNumberGenerator(MedicalSupplyDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
        {
            var nextValue = await _context.Database
                .SqlQuery<int>($"SELECT NEXT VALUE FOR RequestNumberSequence AS Value")
                .SingleAsync(cancellationToken);

            var year = DateTime.UtcNow.Year;

            return $"SR-{year}-{nextValue:D6}";
        }
    }
}
