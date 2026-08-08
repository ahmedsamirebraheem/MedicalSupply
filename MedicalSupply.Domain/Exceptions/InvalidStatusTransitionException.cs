using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalSupply.Domain.Exceptions
{
    public class InvalidStatusTransitionException : Exception
    {
        public InvalidStatusTransitionException(string message) : base(message) { }
    }
}
