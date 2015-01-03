using System;
using System.Collections.Generic;

namespace Thinject
{
    public class Container : IContainer
    {
        private readonly MultiValueDictionary<Type, IRegistration> _registrations = new MultiValueDictionary<Type, IRegistration>();

        private readonly IActivator _activator = new SystemActivatorAdapter();

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

        private class SystemActivatorAdapter : IActivator
        {
            public object ActivateInstance(Type type)
            {
                return Activator.CreateInstance(type);
            }
        }
    }
}
