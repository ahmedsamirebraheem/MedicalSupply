using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Exceptions
{
    public class BudgetExceededException : Exception
    {
        public BudgetExceededException(string message) : base(message) { }
    }
}
