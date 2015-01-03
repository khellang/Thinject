using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Thinject
{
    internal static class ReflectionExtensions
    {
        private static readonly MethodInfo CastMethod = typeof(Enumerable).GetRuntimeMethod("Cast", new[] { typeof(IEnumerable) });

        public static IEnumerable<ConstructorInfo> GetDeclaredConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors;
        }

        public static bool TryGetGenericCollectionArgument(this Type type, out Type genericArgumentType)
        {
            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                genericArgumentType = typeInfo.GenericTypeArguments.First();
                return true;
            };

            genericArgumentType = null;
            return false;
        }

        public static object Cast(this IEnumerable<object> values, Type targetType)
        {
            // TODO: Cache generic methods?
            return CastMethod.MakeGenericMethod(targetType).Invoke(null, new object[] { values });
        }
    }
}