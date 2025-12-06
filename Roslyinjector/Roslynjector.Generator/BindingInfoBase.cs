using Microsoft.CodeAnalysis;

namespace Roslynjector.Generator;

public abstract record BindingInfoBase(
    ITypeSymbol Service, 
    ServiceLifetime Lifetime);