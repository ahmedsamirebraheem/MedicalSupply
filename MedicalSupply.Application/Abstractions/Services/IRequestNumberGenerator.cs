using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.Abstractions.Services
{
    public interface IRequestNumberGenerator
    {
        Task<string> GenerateAsync(CancellationToken cancellationToken = default);
    }
}
