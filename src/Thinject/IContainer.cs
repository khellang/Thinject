using System;

namespace Thinject
{
    public interface IContainer : IResolver, IDisposable
    {
        void RegisterInstance(Type serviceType, object instance);

        void RegisterType(Type serviceType, Type concreteType, Lifetime lifetime);

        void Register<T>(Func<IResolver, T> factory, Lifetime lifetime);

        IContainer CreateChildContainer();
    }
}