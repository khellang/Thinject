using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinject
{
    public interface IContainer
    {
        void Register(Type serviceType, Type concreteType, Lifetime lifetime);

        IEnumerable<object> ResolveAll(Type serviceType);
    }

    public class Container : IContainer
    {
        private readonly MultiValueDictionary<Type, Registration> _registrations = new MultiValueDictionary<Type, Registration>();

        public void Register(Type serviceType, Type concreteType, Lifetime lifetime)
        {
            _registrations.Add(serviceType, new Registration(concreteType, lifetime));
        }

        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            IReadOnlyCollection<Registration> registrations;
            if (!_registrations.TryGetValue(serviceType, out registrations))
            {
                throw new MissingRegistrationException(serviceType);
            }

            foreach (var registration in registrations)
            {
                yield return ActivateInstance(registration);
            }
        }

        private static object ActivateInstance(Registration registration)
        {
            if (registration.Lifetime == Lifetime.Singleton)
            {
                if (registration.SingletonInstance == null)
                {
                    lock (registration)
                    {
                        if (registration.SingletonInstance == null)
                        {
                            registration.SingletonInstance = ActivateInstance(registration.ConcreteType);
                        }
                    }
                }

                return registration.SingletonInstance;
            }

            return ActivateInstance(registration.ConcreteType);
        }

        private static object ActivateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        private class Registration
        {
            public Registration(Type concreteType, Lifetime lifetime)
            {
                ConcreteType = concreteType;
                Lifetime = lifetime;
            }

            public Type ConcreteType { get; private set; }

            public Lifetime Lifetime { get; private set; }

            public object SingletonInstance { get; set; }
        }
    }

    public enum Lifetime
    {
        Transient,

        Singleton
    }

    public static class ContainerExtensions
    {
        public static void Register<T>(this IContainer container)
        {
            Register<T>(container, Lifetime.Transient);
        }

        public static void Register<T>(this IContainer container, Lifetime lifetime)
        {
            container.Register(typeof(T), lifetime);
        }

        public static void Register(this IContainer container, Type type)
        {
            Register(container, type, Lifetime.Transient);
        }

        public static void Register(this IContainer container, Type type, Lifetime lifetime)
        {
            container.Register(type, type, lifetime);
        }

        public static void Register<TService, TConcrete>(this IContainer container) where TConcrete : TService
        {
            Register<TService, TConcrete>(container, Lifetime.Transient);
        }

        public static void Register<TService, TConcrete>(this IContainer container, Lifetime lifetime) where TConcrete : TService
        {
            container.Register(typeof(TService), typeof(TConcrete), lifetime);
        }

        public static TService Resolve<TService>(this IContainer container)
        {
            return container.ResolveAll<TService>().First();
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IContainer container)
        {
            return container.ResolveAll(typeof(TService)).Cast<TService>();
        }
    }

    public class ResolutionException : Exception
    {
        public ResolutionException(string message) : base(message)
        {
        }
    }

    public class MissingRegistrationException : ResolutionException
    {
        public MissingRegistrationException(Type type)
            : base(string.Format("Could not find any registrations for type '{0}'.", type.FullName))
        {
        }
    }
}
