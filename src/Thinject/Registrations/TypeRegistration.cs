using System;

namespace Thinject
{
    internal class TypeRegistration : IRegistration, IDisposable
    {
        private readonly object _padlock = new object();

        private readonly Type _serviceType;

        private readonly Type _concreteType;

        private readonly Lifetime _lifetime;

        private object _instance;

        public TypeRegistration(Type serviceType, Type concreteType, Lifetime lifetime)
        {
            _serviceType = serviceType;
            _concreteType = concreteType;
            _lifetime = lifetime;
        }

        public Type ServiceType
        {
            get { return _serviceType; }
        }

        public object ResolveInstance(IActivator activator)
        {
            if (_lifetime == Lifetime.Transient)
            {
                return activator.ActivateInstance(_concreteType);
            }

            if (_instance == null)
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = activator.ActivateInstance(_concreteType);
                    }
                }
            }

            return _instance;
        }

        public RegistrationValidationResult Validate()
        {
            var result = new RegistrationValidationResult();

            if (!_serviceType.IsAssignableFrom(_concreteType))
            {
                result.AddError("Concrete type '{0}' must be assignable to type '{1}'.", _concreteType.FullName, _serviceType.FullName);
            }

            if (!_concreteType.CanBeConstructed())
            {
                result.AddError("Concrete type '{0}' must not be an interface or abstract class.", _concreteType.FullName);
            }

            return result;
        }

        public void Dispose()
        {
            var disposable = _instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}