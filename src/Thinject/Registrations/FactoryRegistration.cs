using System;

namespace Thinject
{
    internal class FactoryRegistration<T> : TypeRegistration
    {
        public FactoryRegistration(Type serviceType, Func<IResolver, T> factory, Lifetime lifetime)
            : base(serviceType, lifetime)
        {
            Factory = factory;
        }

        public Func<IResolver, T> Factory { get; private set; }

        public override RegistrationValidationResult Validate()
        {
            return new RegistrationValidationResult();
        }

        protected override object ActivateInstance(IActivator activator)
        {
            return Factory.Invoke(activator);
        }
    }
}