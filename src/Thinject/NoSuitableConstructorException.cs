using System;

namespace Thinject
{
    public class NoSuitableConstructorException : ResolutionException
    {
        public NoSuitableConstructorException(Type type)
            : base(string.Format("No suitable constructor found for type '{0}'.", type.FullName))
        {
        }
    }
}