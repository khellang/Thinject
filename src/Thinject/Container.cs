using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinject
{
    public sealed class Container : IContainer
    {
        private readonly MultiValueDictionary<Type, IRegistration> _registrations = new MultiValueDictionary<Type, IRegistration>();

        public void RegisterInstance(Type serviceType, object instance)
        {
            AddRegistration(new InstanceRegistration(serviceType, instance));
        }

        public void RegisterType(Type serviceType, Type concreteType, Lifetime lifetime)
        {
            AddRegistration(new TypeRegistration(serviceType, concreteType, lifetime));
        }

        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            return ResolveInternal(serviceType).ToList();
        }

        private void AddRegistration(IRegistration registration)
        {
            var result = registration.Validate();

            if (!result.IsValid)
            {
                throw new InvalidRegistrationException(result.Errors);
            }

            _registrations.Add(registration.ServiceType, registration);
        }

        private IEnumerable<object> ResolveInternal(Type serviceType)
        {
            IReadOnlyCollection<IRegistration> registrations;
            if (!_registrations.TryGetValue(serviceType, out registrations))
            {
                throw new MissingRegistrationException(serviceType);
            }

            var activator = new Activator(this);

            foreach (var registration in registrations)
            {
                yield return registration.ResolveInstance(activator);
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
    }
}
