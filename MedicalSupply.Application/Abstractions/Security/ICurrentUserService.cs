using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Application.Abstractions.Security
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Email { get; }
        IReadOnlyList<string> Roles { get; }

        bool IsInRole(string role);
    }
}
