using System.Collections.Generic;
using System.Linq;

namespace Thinject
{
    public static class ResolverExtensions
    {
        public static TService Resolve<TService>(this IResolver container) where TService : class
        {
            return (TService) container.Resolve(typeof(TService));
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IResolver container) where TService : class
        {
            return container.ResolveAll(typeof(TService)).Cast<TService>();
        }
    }
}