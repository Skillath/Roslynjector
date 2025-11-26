using System;

namespace Roslynjector.Runtime;

public partial class CompileTimeServiceProvider : IServiceProvider
{
    /*public object? GetService(Type t)
    {
        return TryResolveCore(t, out var inst) ? inst : null;
    }

    public object GetRequiredService(Type t)
    {
        return TryResolveCore(t, out var inst)
            ? inst
            : throw new InvalidOperationException($"No service for '{t.FullName}'.");
    }

    public T GetRequiredService<T>()
    {
        return (T)GetRequiredService(typeof(T));
    }

    partial bool TryResolveCore(Type serviceType, out object instance); // generated*/
    public object GetService(Type serviceType)
    {
        throw new NotImplementedException();
    }
}