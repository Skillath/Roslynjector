using Microsoft.CodeAnalysis;

namespace Roslynjector.Generator;

public sealed record FactoryBindingInfo(
    ITypeSymbol Service, 
    string FactoryText, 
    ServiceLifetime Lifetime)
    : BindingInfoBase(
        Service, 
        Lifetime);