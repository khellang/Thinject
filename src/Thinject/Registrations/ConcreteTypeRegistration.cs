using System;

namespace Thinject
{
    internal class ConcreteTypeRegistration : TypeRegistration
    {
        public ConcreteTypeRegistration(Type serviceType, Type concreteType, Lifetime lifetime) : base(serviceType, lifetime)
        {
            ConcreteType = concreteType;
        }

        public Type ConcreteType { get; private set; }

        protected override bool IsExternallyOwned
        {
            get { return false; }
        }

        public override RegistrationValidationResult Validate()
        {
            var result = new RegistrationValidationResult();

            if (!ServiceType.IsAssignableFrom(ConcreteType))
            {
                result.AddError("Concrete type '{0}' must be assignable to type '{1}'.", ConcreteType.FullName, ServiceType.FullName);
            }

            if (!ConcreteType.CanBeConstructed())
            {
                result.AddError("Concrete type '{0}' must not be an interface or abstract class.", ConcreteType.FullName);
            }

            return result;
        }

        protected override object ActivateInstance(IActivator activator)
        {
            return activator.ActivateInstance(ConcreteType);
        }
    }
}