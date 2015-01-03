using System;

namespace Thinject
{
    internal class InstanceRegistration : IRegistration
    {
        private readonly object _instance;

        private readonly Type _serviceType;

        public InstanceRegistration(Type serviceType, object instance)
        {
            _serviceType = serviceType;
            _instance = instance;
        }

        public Type ServiceType
        {
            get { return _serviceType; }
        }

        public object ResolveInstance(IActivator activator)
        {
            return _instance;
        }

        public RegistrationValidationResult Validate()
        {
            var result = new RegistrationValidationResult();

            var instanceType = _instance.GetType();

            if (!_serviceType.IsAssignableFrom(instanceType))
            {
                result.AddError("Instance of type '{0}' must be assignable to type '{1}'.", instanceType.FullName, _serviceType.FullName);
            }

            return result;
        }
    }
}