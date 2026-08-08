using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.Abstractions.Security
{
    public interface ITokenGenerator
    {
        string GenerateToken(string userId, string email, IReadOnlyList<string> roles);
    }
}
