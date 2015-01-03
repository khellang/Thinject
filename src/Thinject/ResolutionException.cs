using System;

namespace Thinject
{
    public class ResolutionException : Exception
    {
        public ResolutionException(string message) : base(message)
        {
        }
    }
}