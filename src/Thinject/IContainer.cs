using System;
using System.Collections.Generic;

namespace Thinject
{
    public interface IContainer
    {
        void RegisterInstance(Type serviceType, object instance);

        void RegisterType(Type serviceType, Type concreteType, Lifetime lifetime);

        IEnumerable<object> ResolveAll(Type serviceType);
    }
}