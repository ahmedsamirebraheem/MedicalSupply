using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Exceptions
{
    internal class BudgetExceededException : Exception
    {
        public BudgetExceededException(string message) : base(message) { }
    }
}
