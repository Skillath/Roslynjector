using System;
using Microsoft.Extensions.DependencyInjection;

public interface IFoo {}
public sealed class Foo : IFoo { public Foo(IBar b) {} }

public interface IBar {}
public sealed class Bar : IBar {}

static class Bootstrap
{
    static void Configure()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IFoo, Foo>();
        services.AddTransient<IBar, Bar>();
        services.AddScoped<Random>(sp => new Random(7));
        //services.AddTransient(typeof(Uri), sp => new Uri("https://example.com"));
    }
}