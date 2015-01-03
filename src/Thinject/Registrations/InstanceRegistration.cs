using System;

namespace Thinject
{
    internal class InstanceRegistration : TypeRegistration
    {
        public InstanceRegistration(Type serviceType, object instance) : base(serviceType, Lifetime.Singleton)
        {
            Instance = instance;
        }

        protected override bool IsExternallyOwned
        {
            get { return true; }
        }

        public override RegistrationValidationResult Validate()
        {
            var result = new RegistrationValidationResult();

            var instanceType = Instance.GetType();

            if (!ServiceType.IsAssignableFrom(instanceType))
            {
                result.AddError("Instance of type '{0}' must be assignable to type '{1}'.", instanceType.FullName, ServiceType.FullName);
            }

            return result;
        }

        protected override object ActivateInstance(IActivator activator)
        {
            return Instance;
        }
    }
}