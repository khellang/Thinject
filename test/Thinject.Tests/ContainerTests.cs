using Xunit;

namespace Thinject.Tests
{
    public class ContainerTests
    {
        public class RegisterType
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldNotThrowWhenRegisteringSingleType()
            {
                _container.RegisterType<IFoo, Foo>();
            }

            [Fact]
            public void ShouldNotThrowWhenRegisteringMultipleType()
            {
                _container.RegisterType<IFoo, Foo>();
                _container.RegisterType<IFoo, FooBar>();
            }
        }

        public class RegisterInstance
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldNotThrowWhenRegisteringInstance()
            {
                _container.RegisterInstance<IFoo>(new Foo());
            }
        }

        public class Resolve
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldReturnInstanceWithDefaultConstructor()
            {
                _container.RegisterType<IDefaultConstructor, DefaultConstructor>();

                var instance = _container.Resolve<IDefaultConstructor>();

                Assert.True(instance.WasCalled);
            }

            [Fact]
            public void ShouldResolveTransientInstances()
            {
                _container.RegisterType<IFoo, Foo>(Lifetime.Transient);

                var a = _container.Resolve<IFoo>();
                var b = _container.Resolve<IFoo>();

                Assert.NotSame(a, b);
            }

            [Fact]
            public void ShouldResolveSingletonInstances()
            {
                _container.RegisterType<IFoo, Foo>(Lifetime.Singleton);

                var a = _container.Resolve<IFoo>();
                var b = _container.Resolve<IFoo>();

                Assert.Same(a, b);
            }

            [Fact]
            public void ShouldResolveRegisteredSingletonInstance()
            {
                var a = new Foo();

                _container.RegisterInstance<IFoo>(a);

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