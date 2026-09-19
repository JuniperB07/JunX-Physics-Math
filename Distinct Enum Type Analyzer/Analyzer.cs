using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace JunX.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DistinctEnumTypesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "UNIT001";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Generic Enum Types must be distinct",
            messageFormat: "Type parameters '{0}' and '{1}' in '{2}' cannot be the same enum type '{3}'",
            category: "TypeSafety",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeGenericObjectCreation, SyntaxKind.GenericName);
        }

        private static void AnalyzeGenericObjectCreation(SyntaxNodeAnalysisContext context)
        {
            var genericNameSyntax = (GenericNameSyntax)context.Node;
            var typeSymbol = context.SemanticModel.GetTypeInfo(genericNameSyntax).Type as INamedTypeSymbol;

            if (typeSymbol == null || !typeSymbol.IsGenericType)
                return;

            var unboundType = typeSymbol.ConstructedFrom;
            var attribute = unboundType.GetAttributes()
                .FirstOrDefault(ad => ad.AttributeClass?.Name == "DistinctEnumTypesAttribute");

            if(attribute != null)
            {
                int idx1 = attribute.ConstructorArguments.Length > 0 ? (int)attribute.ConstructorArguments[0].Value! : 1;
                int idx2 = attribute.ConstructorArguments.Length > 1 ? (int)attribute.ConstructorArguments[1].Value! : 3;

                if(typeSymbol.TypeArguments.Length > Math.Max(idx1, idx2))
                {
                    ITypeSymbol en1 = typeSymbol.TypeArguments[idx1];
                    ITypeSymbol en2 = typeSymbol.TypeArguments[idx2];

                    if(SymbolEqualityComparer.Default.Equals(en1, en2))
                    {
                        var diagnostic = Diagnostic.Create(
                            Rule,
                            genericNameSyntax.GetLocation(),
                            unboundType.TypeParameters[idx1].Name,
                            unboundType.TypeParameters[idx2].Name,
                            typeSymbol.Name,
                            en1.ToDisplayString());

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
