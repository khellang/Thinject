namespace Thinject
{
    internal class InstanceRegistration : IRegistration
    {
        private readonly object _instance;

        public InstanceRegistration(object instance)
        {
            _instance = instance;
        }

        public object ResolveInstance(IActivator activator)
        {
            return _instance;
        }
    }
}