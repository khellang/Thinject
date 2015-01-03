using System;
using System.Collections.Generic;

namespace Thinject
{
    public interface IResolver
    {
        object Resolve(Type serviceType);

        IEnumerable<object> ResolveAll(Type serviceType);
    }
}