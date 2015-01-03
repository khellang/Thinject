using System;

namespace Thinject
{
    internal interface IActivator
    {
        object ActivateInstance(Type type);
    }
}