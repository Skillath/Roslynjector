using Microsoft.Extensions.DependencyInjection;

namespace Roslynjector;

public static class CompileTimeServices
{
    public static IServiceCollection AddSingleton<TService, TImpl>(this IServiceCollection serviceCollection)
        where TImpl : TService
    {
        return serviceCollection;
    }

    public static IServiceCollection AddTransient<TService, TImpl>(this IServiceCollection serviceCollection)
        where TImpl : TService
    {
        return serviceCollection;
    }

    public static IServiceCollection AddSingleton<TService>(
        this IServiceCollection serviceCollection, 
        Func<IServiceProvider, TService> factory)
    {
        return serviceCollection;
    }

    public static IServiceCollection AddTransient<TService>(
        this IServiceCollection serviceCollection, 
        Func<IServiceProvider, TService> factory)
    {
        return serviceCollection;
    }

    // Non-generic overloads:
    public static IServiceCollection AddSingleton(
        this IServiceCollection serviceCollection, 
        Type service, 
        Type impl)
    {
        return serviceCollection;
    }

    public static IServiceCollection AddTransient(
        this IServiceCollection serviceCollection, 
        Type service, 
        Type impl)
    {
        return serviceCollection;
    }

    public static IServiceCollection AddSingleton(
        this IServiceCollection serviceCollection, 
        Type service, 
        Func<IServiceProvider, object> factory)
    {
        return serviceCollection;
    }

    public static IServiceCollection AddTransient(
        this IServiceCollection serviceCollection, 
        Type service, 
        Func<IServiceProvider, object> factory)
    {
        return serviceCollection;
    }
}