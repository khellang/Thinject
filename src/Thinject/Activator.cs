using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Thinject
{
    internal class Activator : IActivator
    {
        private static readonly MethodInfo ResolveMethod = typeof(IResolver).GetRuntimeMethod("Resolve", new[] { typeof(Type) });

        private readonly object _padlock = new object();

        private readonly IDictionary<Type, ConstructorInfo> _constructorCache;

        private readonly IContainer _container;

        public Activator(IContainer container)
        {
            _container = container;
            _constructorCache = new Dictionary<Type, ConstructorInfo>();
        }

        public object ActivateInstance(Type type)
        {
            ConstructorInfo cachedConstructor;
            if (TryGetCachedConstructor(type, out cachedConstructor))
            {
                object[] arguments;
                if (TryResolveParameters(cachedConstructor.GetParameters(), out arguments))
                {
                    return cachedConstructor.Invoke(arguments.ToArray());
                }
            }

            var constructors = GetConstructorDictionary(type).OrderByDescending(x => x.Value.Count);

            foreach (var constructor in constructors)
            {
                object[] arguments;
                if (TryResolveParameters(constructor.Value, out arguments))
                {
                    var instance = constructor.Key.Invoke(arguments.ToArray());

                    lock (_padlock)
                    {
                        _constructorCache.Add(type, constructor.Key);
                    }

                    return instance;
                }
            }

            throw new NoSuitableConstructorException(type);
        }

        private bool TryGetCachedConstructor(Type type, out ConstructorInfo cachedConstructor)
        {
            lock (_padlock)
            {
                return _constructorCache.TryGetValue(type, out cachedConstructor);
            }
        }

        public object Resolve(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        public IEnumerable<object> ResolveAll(Type serviceType)
        {
            return _container.ResolveAll(serviceType);
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
                    }
                    else if (parameterType.TryGetGenericFuncArgument(out argumentType))
                    {
                        var methodCallExpression = Expression.Call(
                            Expression.Constant(_container),
                            ResolveMethod,
                            Expression.Constant(argumentType));

                        var convertExpression = Expression.Convert(
                            methodCallExpression,
                            argumentType);

                        args.Add(Expression.Lambda(convertExpression).Compile());
                    }
                    else
                    {
                        args.Add(_container.Resolve(parameterType));
                    }
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