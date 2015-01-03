using System;
using System.Collections.Generic;
using System.Linq;

namespace Thinject
{
    public sealed class Container : IContainer
    {
        private readonly MultiValueDictionary<Type, IRegistration> _registrations;

        private readonly IList<IContainer> _children;

        private readonly Container _parent;

        public Container()
        {
            _registrations = new MultiValueDictionary<Type, IRegistration>();
            _children = new List<IContainer>();
        }

        private Container(Container parent) : this()
        {
            _parent = parent;
        }

        public void RegisterInstance(Type serviceType, object instance)
        {
            AddRegistration(new InstanceRegistration(serviceType, instance));
        }

        public void RegisterType(Type serviceType, Type concreteType, Lifetime lifetime)
        {
            AddRegistration(new TypeRegistration(serviceType, concreteType, lifetime));
        }

        public object Resolve(Type serviceType)
        {
            return ResolveInternal(serviceType).First();
        }

        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            return ResolveInternal(serviceType).ToList();
        }

        public IContainer CreateChildContainer()
        {
            var child = new Container(this);

            _children.Add(child);

            return child;
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
                if (_parent == null)
                {
                    throw new MissingRegistrationException(serviceType);
                }

                foreach (var instance in _parent.ResolveInternal(serviceType))
                {
                    yield return instance;
                }

                yield break;
            }

            var activator = new Activator(this);

            foreach (var registration in registrations)
            {
                yield return registration.ResolveInstance(activator);
            }
        }

        public void Dispose()
        {
            foreach (var child in _children)
            {
                child.Dispose();
            }

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
