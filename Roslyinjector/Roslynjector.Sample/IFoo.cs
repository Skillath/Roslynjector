using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Roslynjector.Sample;

public interface IFoo
{
    
}

public sealed class Foo : IFoo
{
    private readonly IBar _bar;

    public Foo(IBar bar)
    {
        _bar = bar;
    }
}

public interface IBar
{
    
}

public sealed class Bar : IBar
{
    
}

public sealed class Far
{
    private readonly IFoo _foo;
    private readonly IBar _bar;

    public Far(IFoo foo, [FromKeyedServices("Test")] IBar bar)
    {
        _foo = foo;
        _bar = bar;
    }
}

public sealed class Boo
{
    private readonly IFoo _foo;

    public Boo(IFoo foo)
    {
        _foo = foo;
    }
}

public static class Bootstrap
{
    private static void Configure()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IFoo, Foo>();
        services.AddTransient<IBar, Bar>();
        services.AddTransient<Far>();
        services.AddTransient(typeof(Boo));
        services.AddScoped<Random>(_ => new Random(7));
        services.AddScoped(_ => "hello");
        
        services.AddSingleton<List<int>>(new List<int>());
        services.AddSingleton(new List<string>());
        services.AddTransient(typeof(Uri), _ => new Uri("https://example.com"));
    }
}