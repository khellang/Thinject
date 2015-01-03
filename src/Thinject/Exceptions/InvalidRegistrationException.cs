using System;
using System.Collections.Generic;

namespace Thinject
{
    public class InvalidRegistrationException : RegistrationException
    {
        public InvalidRegistrationException(IEnumerable<string> validationErrors)
            : base(string.Concat("Invalid registration:\n", string.Join(Environment.NewLine, validationErrors)))
        {
        }
    }
}