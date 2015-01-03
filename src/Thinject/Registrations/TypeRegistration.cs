using System;

namespace Thinject
{
    internal abstract class TypeRegistration : IRegistration, IDisposable
    {
        private readonly object _padlock = new object();

        protected TypeRegistration(Type serviceType, Lifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        public Type ServiceType { get; private set; }

        public Lifetime Lifetime { get; private set; }

        public object Instance { get; protected set; }

        protected abstract bool IsExternallyOwned { get; }

        public object ResolveInstance(IActivator activator)
        {
            if (Lifetime == Lifetime.Transient)
            {
                return ActivateInstance(activator);
            }

            if (Instance == null)
            {
                lock (_padlock)
                {
                    if (Instance == null)
                    {
                        Instance = ActivateInstance(activator);
                    }
                }
            }

            return Instance;
        }

        public void Dispose()
        {
            if (IsExternallyOwned)
            {
                return;
            }

            var disposable = Instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public virtual RegistrationValidationResult Validate()
        {
            return new RegistrationValidationResult();
        }

        protected abstract object ActivateInstance(IActivator activator);
    }
}