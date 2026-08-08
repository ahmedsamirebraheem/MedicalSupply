using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Infrastructure.Identity
{
    public class HardcodedUser
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string Role { get; init; } = null!;
    }
}
