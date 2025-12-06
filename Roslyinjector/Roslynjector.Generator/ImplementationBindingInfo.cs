using Microsoft.CodeAnalysis;

namespace Roslynjector.Generator;

public sealed record ImplementationBindingInfo(
    ITypeSymbol Service, 
    ITypeSymbol Implementation, 
    ServiceLifetime Lifetime)
    : BindingInfoBase(
        Implementation, 
        Lifetime);