using System;
using Microsoft.Extensions.DependencyInjection;

namespace Roslynjector.Sample;

public interface IFoo
{
    
}

public sealed class Foo : IFoo
{
    public Foo(IBar b)
    {
        
    }
}

public interface IBar
{
    
}

public sealed class Bar : IBar
{
    
}

static class Bootstrap
{
    static void Configure()
    {
        var services = new ServiceCollection();
        ServiceCollectionServiceExtensions.AddSingleton<IFoo, Foo>(services);
        ServiceCollectionServiceExtensions.AddTransient<IBar, Bar>(services);
        services.AddScoped<Random>(sp => new Random(7));
        ServiceCollectionServiceExtensions.AddTransient(services, typeof(Uri), sp => new Uri("https://example.com"));
        
    }
}