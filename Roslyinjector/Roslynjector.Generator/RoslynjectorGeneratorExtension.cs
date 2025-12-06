using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynjector.Generator;

public static class RoslynjectorGeneratorExtension
{
    public const string AddSingletonMethodName = "AddSingleton";
    public const string AddTransientMethodName = "AddTransient";
    public const string AddScopedMethodName = "AddScoped";
    
    public static bool IsAddCallCandidate(this SyntaxNode node)
    {
        if (node is not InvocationExpressionSyntax inv) 
            return false;

        // Normal:   services.AddSingleton(...)
        if (inv.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            var name = memberAccessExpressionSyntax.Name switch
            {
                GenericNameSyntax genericNameSyntax => genericNameSyntax.Identifier.ValueText,
                IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier.ValueText,
                _ => null
            };
            
            return name is AddSingletonMethodName or AddTransientMethodName or AddScopedMethodName;
        }

        // Conditional access: services?.AddSingleton(...)
        if (inv.Expression is MemberBindingExpressionSyntax { Name: SimpleNameSyntax simpleNameSyntax })
        {
            var id = simpleNameSyntax switch
            {
                GenericNameSyntax genericNameSyntax => genericNameSyntax.Identifier.ValueText,
                IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier.ValueText,
                _ => null
            };
            
            return id is AddSingletonMethodName or AddTransientMethodName or AddScopedMethodName;
        }

        return false;
    }

    public static BindingInfoBase? TransformAddCall(this GeneratorSyntaxContext context)
    {
        var inv = (InvocationExpressionSyntax)context.Node;
        var model = context.SemanticModel;

        if (model.GetSymbolInfo(inv).Symbol is not IMethodSymbol methodSymbol) 
            return null;

        var lifetime = methodSymbol.Name switch
        {
            AddSingletonMethodName => ServiceLifetime.Singleton,
            AddScopedMethodName => ServiceLifetime.Scoped,
            _ => ServiceLifetime.Transient
        };

        // CASE 1: Generic impl-based: AddX<TService,TImpl>()
        if (methodSymbol.TypeArguments.Length == 2)
        {
            if (methodSymbol.TypeArguments[0] is not INamedTypeSymbol service 
                || methodSymbol.TypeArguments[1] is not INamedTypeSymbol implementation) 
                return null;
            
            return new ImplementationBindingInfo(service, implementation, lifetime);
        }
        
        // CASE 1.1: Generic impl-based: AddX<TService>()
        if (methodSymbol.TypeArguments.Length == 1 
            && inv.ArgumentList.Arguments.Count == 0)
        {
            if (methodSymbol.TypeArguments[0] is not INamedTypeSymbol service) 
                return null;
            
            return new ImplementationBindingInfo(service, service, lifetime);
        }

        // CASE 2: Generic factory: AddX<TService>(Func<IServiceProvider,TService> factory)
        if (methodSymbol.TypeArguments.Length == 1 
            && inv.ArgumentList.Arguments.Count == 1)
        {
            if (methodSymbol.TypeArguments[0] is not INamedTypeSymbol service) 
                return null;
            
            var factoryText = inv.ArgumentList.Arguments[0].ToFullString();
            return new FactoryBindingInfo(service, factoryText, lifetime);
        }

        // CASE 3: Non-generic impl-based: AddX(typeof(Svc), typeof(Impl))
        if (methodSymbol.TypeArguments.Length == 0 
            && inv.ArgumentList.Arguments.Count == 2)
        {
            var argument0 = inv.ArgumentList.Arguments[0].Expression;
            var argument1 = inv.ArgumentList.Arguments[1].Expression;

            if (argument0 is TypeOfExpressionSyntax typeOfExpressionSyntax0 
                && argument1 is TypeOfExpressionSyntax typeOfExpressionSyntax1)
            {
                var service = model.GetTypeInfo(typeOfExpressionSyntax0.Type).Type as INamedTypeSymbol;
                var implementation = model.GetTypeInfo(typeOfExpressionSyntax1.Type).Type as INamedTypeSymbol;
                
                if (service is not null && implementation is not null)
                    return new ImplementationBindingInfo(service, implementation, lifetime);
            }
        }
        
        // CASE 3.1: Non-generic impl-based: AddX(typeof(Svc))
        if (methodSymbol.TypeArguments.Length == 0 && 
            inv.ArgumentList.Arguments.Count == 1)
        {
            var argument0 = inv.ArgumentList.Arguments[0].Expression;

            if (argument0 is TypeOfExpressionSyntax typeOfExpressionSyntax)
            {
                var service = model.GetTypeInfo(typeOfExpressionSyntax.Type).Type as INamedTypeSymbol;
                
                if (service is not null )
                    return new ImplementationBindingInfo(service, service, lifetime);
            }
        }

        // CASE 4: Non-generic factory: AddX(typeof(Svc), factory)
        if (methodSymbol.TypeArguments.Length == 0 && inv.ArgumentList.Arguments.Count == 2)
        {
            var argument0 = inv.ArgumentList.Arguments[0].Expression;
            var argument1 = inv.ArgumentList.Arguments[1];

            if (argument0 is not TypeOfExpressionSyntax t0) 
                return null;

            if (model.GetTypeInfo(t0.Type).Type is not INamedTypeSymbol service) 
                return null;
                
            var factoryText = argument1.ToFullString();
            return new FactoryBindingInfo(service, factoryText, lifetime);
        }

        return null;
    }
}