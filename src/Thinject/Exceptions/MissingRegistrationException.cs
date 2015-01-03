using System;

namespace Thinject
{
    public class MissingRegistrationException : ResolutionException
    {
        public MissingRegistrationException(Type type)
            : base(string.Format("Could not find any registrations for type '{0}'.", type.FullName))
        {
        }
    }
}