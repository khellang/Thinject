namespace Thinject
{
    internal interface IRegistration
    {
        object ResolveInstance(IActivator activator);
    }
}