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
            var sequenceParam = new Microsoft.Data.SqlClient.SqlParameter
            {
                ParameterName = "@result",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "SET @result = NEXT VALUE FOR RequestNumberSequence",
                new object[] { sequenceParam },
                cancellationToken);

            var nextValue = (int)sequenceParam.Value;

            var year = DateTime.UtcNow.Year;

            return $"SR-{year}-{nextValue:D6}";
        }
    }
}
