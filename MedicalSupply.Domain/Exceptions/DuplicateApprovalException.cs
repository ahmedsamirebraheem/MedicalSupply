using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Exceptions
{
    internal class DuplicateApprovalException : Exception
    {
        public DuplicateApprovalException(string message) : base(message) { }
    }
}
