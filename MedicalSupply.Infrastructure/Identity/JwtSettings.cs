using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Identity
{
    public class JwtSettings
    {
        public string SecretKey { get; init; } = null!;
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
        public int ExpiryMinutes { get; init; }
    }
}
