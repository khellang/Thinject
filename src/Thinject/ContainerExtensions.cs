using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinject
{
    public static class ContainerExtensions
    {
        public static void RegisterInstance<T>(this IContainer container, T instance)
        {
            container.RegisterInstance(typeof(T), instance);
        }

        public static void RegisterTypes<T>(this IContainer container, IEnumerable<Type> types)
        {
            container.RegisterTypes<T>(types, Lifetime.Transient);
        }

        public static void RegisterTypes<T>(this IContainer container, IEnumerable<Type> types, Lifetime lifetime)
        {
            foreach (var type in types)
            {
                container.RegisterType(typeof(T), type, lifetime);
            }
        }

        public static void RegisterType<T>(this IContainer container)
        {
            container.RegisterType<T>(Lifetime.Transient);
        }

        public static void RegisterType<T>(this IContainer container, Lifetime lifetime)
        {
            container.RegisterType(typeof(T), lifetime);
        }

        public static void RegisterType(this IContainer container, Type type)
        {
            container.RegisterType(type, Lifetime.Transient);
        }

        public static void RegisterType(this IContainer container, Type type, Lifetime lifetime)
        {
            container.RegisterType(type, type, lifetime);
        }

        public static void RegisterType<TService, TConcrete>(this IContainer container) where TConcrete : TService
        {
            container.RegisterType<TService, TConcrete>(Lifetime.Transient);
        }

        public static void RegisterType<TService, TConcrete>(this IContainer container, Lifetime lifetime) where TConcrete : TService
        {
            container.RegisterType(typeof(TService), typeof(TConcrete), lifetime);
        }

        public static TService Resolve<TService>(this IContainer container)
        {
            return (TService) container.Resolve(typeof(TService));
        }

        public static object Resolve(this IContainer container, Type serviceType)
        {
            return container.ResolveAll(serviceType).First();
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IContainer container)
        {
            return container.ResolveAll(typeof(TService)).Cast<TService>();
        }
    }
}