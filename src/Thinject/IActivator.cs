using System;

namespace Thinject
{
    internal interface IActivator : IResolver
    {
        object ActivateInstance(Type type);
    }
}