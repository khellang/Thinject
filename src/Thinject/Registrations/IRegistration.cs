using System;

namespace Thinject
{
    internal interface IRegistration
    {
        Type ServiceType { get; }

        object ResolveInstance(IActivator activator);

        RegistrationValidationResult Validate();
    }
}