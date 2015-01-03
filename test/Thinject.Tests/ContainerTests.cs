using Xunit;

namespace Thinject.Tests
{
    public class ContainerTests
    {
        public class Register
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldNotThrowWhenRegisteringSingleType()
            {
                _container.Register<IFoo, Foo>();
            }

            [Fact]
            public void ShouldNotThrowWhenRegisteringMultipleType()
            {
                _container.Register<IFoo, Foo>();
                _container.Register<IFoo, FooBar>();
            }
        }

        public class Resolve
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldReturnInstanceWithDefaultConstructor()
            {
                _container.Register<IDefaultConstructor, DefaultConstructor>();

                var instance = _container.Resolve<IDefaultConstructor>();

                Assert.True(instance.WasCalled);
            }

            [Fact]
            public void ShouldResolveTransientInstances()
            {
                _container.Register<IFoo, Foo>(Lifetime.Transient);

                var a = _container.Resolve<IFoo>();
                var b = _container.Resolve<IFoo>();

                Assert.NotSame(a, b);
            }

            [Fact]
            public void ShouldResolveSingletonInstances()
            {
                _container.Register<IFoo, Foo>(Lifetime.Singleton);

                var a = _container.Resolve<IFoo>();
                var b = _container.Resolve<IFoo>();

                Assert.Same(a, b);
            }
        }
    }

    public interface IFoo
    {
    }

    public class Foo : IFoo
    {
    }

    public class FooBar : IFoo
    {
    }

    public interface IDefaultConstructor
    {
        bool WasCalled { get; }
    }

    public class DefaultConstructor : IDefaultConstructor
    {
        public DefaultConstructor()
        {
            WasCalled = true;
        }

        public bool WasCalled { get; private set; }
    }
}