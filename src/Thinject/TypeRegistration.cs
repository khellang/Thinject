using System;

namespace Thinject
{
    internal class TypeRegistration : IRegistration
    {
        private readonly object _padlock = new object();

        private readonly Type _concreteType;

        private readonly Lifetime _lifetime;

        private object _instance;

        public TypeRegistration(Type concreteType, Lifetime lifetime)
        {
            _concreteType = concreteType;
            _lifetime = lifetime;
        }

        public object ResolveInstance(IActivator activator)
        {
            if (_lifetime == Lifetime.Singleton)
            {
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

            return activator.ActivateInstance(_concreteType);
        }
    }
}