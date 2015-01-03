using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Thinject
{
    internal class Activator : IActivator
    {
        private readonly ConcurrentDictionary<Type, ConstructorInfo> _constructorCache;

        private readonly IContainer _container;

        public Activator(IContainer container)
        {
            _container = container;
            _constructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();
        }

        public object ActivateInstance(Type type)
        {
            ConstructorInfo cachedConstructor;
            if (_constructorCache.TryGetValue(type, out cachedConstructor))
            {
                object[] arguments;
                if (TryResolveParameters(cachedConstructor.GetParameters(), out arguments))
                {
                    return cachedConstructor.Invoke(arguments.ToArray());
                }
            }
            else
            {
                var constructors = GetConstructorDictionary(type).OrderByDescending(x => x.Value.Count);

                foreach (var constructor in constructors)
                {
                    object[] arguments;
                    if (TryResolveParameters(constructor.Value, out arguments))
                    {
                        var instance = constructor.Key.Invoke(arguments.ToArray());

                        _constructorCache.TryAdd(type, constructor.Key);

                        return instance;
                    }
                }
            }

            throw new NoSuitableConstructorException(type);
        }

        private bool TryResolveParameters(IEnumerable<ParameterInfo> parameters, out object[] arguments)
        {
            var args = new List<object>();

            foreach (var parameter in parameters)
            {
                try
                {
                    var parameterType = parameter.ParameterType;

                    Type argumentType;
                    if (parameterType.TryGetGenericCollectionArgument(out argumentType))
                    {
                        args.Add(_container.ResolveAll(argumentType).Cast(argumentType));
                        continue;
                    }

                    args.Add(_container.Resolve(parameterType));
                }
                catch (Exception)
                {
                    arguments = new object[0];
                    return false;
                }
            }

            arguments = args.ToArray();
            return true;
        }

        private static MultiValueDictionary<ConstructorInfo, ParameterInfo> GetConstructorDictionary(Type type)
        {
            var constructors = new MultiValueDictionary<ConstructorInfo, ParameterInfo>();

            foreach (var constructor in type.GetDeclaredConstructors())
            {
                constructors.AddRange(constructor, constructor.GetParameters());
            }

            return constructors;
        }
    }
}