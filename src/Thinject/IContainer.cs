using System;
using System.Collections.Generic;

namespace Thinject
{
    public interface IContainer : IDisposable
    {
        void RegisterInstance(Type serviceType, object instance);

        void RegisterType(Type serviceType, Type concreteType, Lifetime lifetime);

        object Resolve(Type serviceType);

        IEnumerable<object> ResolveAll(Type serviceType);
    }
}