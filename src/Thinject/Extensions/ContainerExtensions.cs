using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinject
{
    public static class ContainerExtensions
    {
        public static void RegisterInstance<TService>(this IContainer container, TService instance) where TService : class
        {
            container.RegisterInstance(typeof(TService), instance);
        }

        public static void RegisterTypes<TService>(this IContainer container, IEnumerable<Type> concreteTypes) where TService : class
        {
            container.RegisterTypes<TService>(concreteTypes, Lifetime.Transient);
        }

        public static void RegisterTypes<TService>(this IContainer container, IEnumerable<Type> concreteTypes, Lifetime lifetime) where TService : class
        {
            foreach (var concreteType in concreteTypes)
            {
                container.RegisterType(typeof(TService), concreteType, lifetime);
            }
        }

        public static void RegisterType<TService>(this IContainer container) where TService : class
        {
            container.RegisterType<TService>(Lifetime.Transient);
        }

        public static void RegisterType<TService>(this IContainer container, Lifetime lifetime) where TService : class
        {
            container.RegisterType(typeof(TService), lifetime);
        }

        public static void RegisterType(this IContainer container, Type concreteType)
        {
            container.RegisterType(concreteType, Lifetime.Transient);
        }

        public static void RegisterType(this IContainer container, Type concreteType, Lifetime lifetime)
        {
            container.RegisterType(concreteType, concreteType, lifetime);
        }

        public static void RegisterType<TService, TConcrete>(this IContainer container)
            where TConcrete : class, TService
            where TService : class
        {
            container.RegisterType<TService, TConcrete>(Lifetime.Transient);
        }

        public static void RegisterType<TService, TConcrete>(this IContainer container, Lifetime lifetime)
            where TConcrete : class, TService
            where TService : class
        {
            container.RegisterType(typeof(TService), typeof(TConcrete), lifetime);
        }

        public static TService Resolve<TService>(this IContainer container) where TService : class
        {
            return (TService) container.Resolve(typeof(TService));
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IContainer container) where TService : class
        {
            return container.ResolveAll(typeof(TService)).Cast<TService>();
        }
    }
}