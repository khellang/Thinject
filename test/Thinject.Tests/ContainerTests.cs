using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Thinject.Tests
{
    public class ContainerTests
    {
        public class RegisterInstance
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldRegisterInstanceSuccessfully()
            {
                _container.RegisterInstance<IFoo>(new Foo());
            }
        }

        public class RegisterType
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldRegisterSingleTypeSuccessfully()
            {
                _container.RegisterType<IFoo, Foo>();
            }

            [Fact]
            public void ShouldRegisterMultipleTypesSuccessfully()
            {
                _container.RegisterType<IFoo, Foo>();
                _container.RegisterType<IFoo, FooBar>();
            }
        }

        public class Register
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldRegisterSingleTypeSuccessfully()
            {
                _container.Register(c => new Foo());
            }

            [Fact]
            public void ShouldRegisterMultipleTypesSuccessfully()
            {
                _container.Register<IFoo>(c => new Foo());
                _container.Register<IBar>(c => new Bar(c.Resolve<IFoo>()));
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

            [Fact]
            public void ShouldResolveInstanceWithConstructorParameters()
            {
                _container.RegisterType<IFoo, Foo>();
                _container.RegisterType<IBar, Bar>();

                var instance = _container.Resolve<IBar>();

                Assert.NotNull(instance);
            }

            [Fact]
            public void ShouldThrowIfMissingRegistration()
            {
                Assert.Throws<MissingRegistrationException>(() => _container.Resolve<IBar>());
            }

            [Fact]
            public void ShouldThrowIfNoSuitableConstructors()
            {
                _container.RegisterType<IBar, Bar>();

                Assert.Throws<NoSuitableConstructorException>(() => _container.Resolve<IBar>());
            }

            [Fact]
            public void ShouldReturnFirstInstanceOfFirstRegistrationIfMultiple()
            {
                _container.RegisterTypes<IFoo>(new[] { typeof(Foo), typeof(FooBar) });

                var instance = _container.Resolve<IFoo>();

                Assert.IsType<Foo>(instance);
            }

            [Fact]
            public void ShouldResolveEnumerableConstructorParameters()
            {
                _container.RegisterType<IFoo, Foo>();
                _container.RegisterType<IFoo, FooBar>();
                _container.RegisterType<EnumerableConstructor>();

                var instance = _container.Resolve<EnumerableConstructor>();

                Assert.Equal(2, instance.Foos.Count);
            }

            [Fact]
            public void ShouldResolveFromParent()
            {
                _container.RegisterType<IFoo, Foo>(Lifetime.Singleton);

                var child = _container.CreateChildContainer();

                var a = _container.Resolve<IFoo>();
                var b = child.Resolve<IFoo>();

                Assert.Same(a, b);
            }

            [Fact]
            public void ShouldResolveInstanceFromFactory()
            {
                _container.Register<IFoo>(c => new Foo());
                _container.Register<IBar>(c => new Bar(c.Resolve<IFoo>()));

                var instance = _container.Resolve<IBar>();

                Assert.NotNull(instance.Foo);
            }
        }

        public class ResolveAll
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldResolveAllRegistrations()
            {
                _container.RegisterTypes<IFoo>(new[] { typeof(Foo), typeof(FooBar) });

                var instances = _container.ResolveAll<IFoo>().ToList();

                Assert.Equal(2, instances.Count);
            }
        }

        public class Dispose
        {
            private readonly IContainer _container = new Container();

            [Fact]
            public void ShouldDisposeContainerActivatedSingletons()
            {
                _container.RegisterType<Foo>(Lifetime.Singleton);

                var instance = _container.Resolve<Foo>();

                _container.Dispose();

                Assert.True(instance.IsDisposed);
            }

            [Fact]
            public void ShouldNotDisposeExternallyActivatedSingletons()
            {
                var instance = new Foo();

                _container.RegisterInstance(instance);

                _container.Dispose();

                Assert.False(instance.IsDisposed);
            }

            [Fact]
            public void ShouldDisposeChildContainers()
            {
                var child = _container.CreateChildContainer();

                child.RegisterType<Foo>(Lifetime.Singleton);

                var instance = child.Resolve<Foo>();

                _container.Dispose();

                Assert.True(instance.IsDisposed);
            }
        }

        public class CreateChildContainer
        {
            [Fact]
            public void ShouldCreateChildContainer()
            {
                var container = new Container();

                container.CreateChildContainer();
            }
        }
    }

    public interface IFoo
    {
    }

    public class Foo : IFoo, IDisposable
    {
        public void Dispose()
        {
            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }
    }

    public interface IBar
    {
        IFoo Foo { get; }
    }

    public class Bar : IBar
    {
        public Bar(IFoo foo)
        {
            Foo = foo;
        }

        public IFoo Foo { get; private set; }
    }

    public class FooBar : IFoo, IBar
    {
        public IFoo Foo { get; private set; }
    }

    public interface IDefaultConstructor : IFoo
    {
        bool WasCalled { get; }
    }

    public class EnumerableConstructor
    {
        public EnumerableConstructor(IEnumerable<IFoo> foos)
        {
            Foos = foos.ToList();
        }

        public IList<IFoo> Foos { get; private set; }
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