using System;
using Roslynjector.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace Roslynjector.Sample;

// 1) Define your service contracts and implementations
public interface IFoo { }
public sealed class Foo : IFoo
{
    public Foo(IBar bar) { /* ... */ }
}

public interface IBar { }
public sealed class Bar : IBar { }

// Open generic example
public interface IRepository<T> { }
public sealed class Repository<T> : IRepository<T> { }

class Program
{
    static void Main()
    {
        // 2) Create a collection (any instance works; it's just a marker for the generator)
        var services = new ServiceCollection();

        // 3) Register services (implementation-based)
        services.AddSingleton<IFoo, Foo>();
        services.AddTransient<IBar, Bar>();

        // 4) Open generics
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

        // 5) Factory-based registrations
        services.AddSingleton<Random>(sp => new Random(42)); // example factory
        services.AddTransient<IBar>(sp => new Bar());       // factory overrides constructor analysis

        // 6) Multiple registrations for the same service (IEnumerable<T>)
        services.AddTransient<IFoo, Foo>();            // imagine another impl:
        // services.AddTransient<IFoo, SpecialFoo>();  // IEnumerable<IFoo> contains both (if added)

        // 7) Use the generated provider at runtime
        var provider = new CompileTimeServiceProvider();

        // Resolve services
        var foo = provider.GetRequiredService<IFoo>();                // returns Foo (singleton)
        var bar = provider.GetRequiredService<IBar>();                // new Bar() each time (transient)
        var repoInt = provider.GetRequiredService<IRepository<int>>();// closed generic
        var allFoos = provider.GetRequiredService<System.Collections.Generic.IEnumerable<IFoo>>();

        Console.WriteLine(foo.GetType().Name);
    }
}