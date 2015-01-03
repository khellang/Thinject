using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Thinject
{
    public sealed class Container : IContainer
    {
        private readonly MultiValueDictionary<Type, IRegistration> _registrations = new MultiValueDictionary<Type, IRegistration>();

        private readonly IActivator _activator;

        public Container()
        {
            _activator = new SystemActivatorAdapter(this);
        }

        public void RegisterInstance(Type serviceType, object instance)
        {
            _registrations.Add(serviceType, new InstanceRegistration(instance));
        }

        public void RegisterType(Type serviceType, Type concreteType, Lifetime lifetime)
        {
            _registrations.Add(serviceType, new TypeRegistration(concreteType, lifetime));
        }

        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            IReadOnlyCollection<IRegistration> registrations;
            if (!_registrations.TryGetValue(serviceType, out registrations))
            {
                throw new MissingRegistrationException(serviceType);
            }

            foreach (var registration in registrations)
            {
                yield return registration.ResolveInstance(_activator);
            }
        }

        public void Dispose()
        {
            foreach (var service in _registrations)
            {
                foreach (var registration in service.Value)
                {
                    var disposable = registration as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        private class SystemActivatorAdapter : IActivator
        {
            private readonly IContainer _container;

            public SystemActivatorAdapter(IContainer container)
            {
                _container = container;
            }

            public object ActivateInstance(Type type)
            {
                var constructors = GetConstructorDictionary(type).OrderByDescending(x => x.Value.Count);

                foreach (var constructor in constructors)
                {
                    object[] arguments;
                    if (TryResolveParameters(constructor.Value, out arguments))
                    {
                        return constructor.Key.Invoke(arguments.ToArray());
                    }
                }

                throw new NoSuitableConstructorException(type);
            }

            private bool TryResolveParameters(IEnumerable<ParameterInfo> parameters, out object[] arguments)
            {
                var args = new List<object>();

                foreach (var parameter in parameters)
                {
                    try
                    {
                        args.Add(_container.Resolve(parameter.ParameterType));
                    }
                    catch (Exception)
                    {
                        arguments = new object[0];
                        return false;
                    }
                }

                arguments = args.ToArray();
                return true;
            }

            private static MultiValueDictionary<ConstructorInfo, ParameterInfo> GetConstructorDictionary(Type type)
            {
                var constructors = new MultiValueDictionary<ConstructorInfo, ParameterInfo>();

                foreach (var constructor in type.GetTypeInfo().DeclaredConstructors)
                {
                    constructors.AddRange(constructor, constructor.GetParameters());
                }

                return constructors;
            }
        }
    }
}
