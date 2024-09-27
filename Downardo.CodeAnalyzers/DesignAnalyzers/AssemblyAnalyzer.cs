using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Downardo.CodeAnalyzers.DesignAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AssemblyAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            new DiagnosticDescriptor(DiagnosticCodes.AssembliesHaveCompanyAttributeAnalyzer, "Assemblies should have a company attribute", "Assemblies should have a company attribute", "Design", DiagnosticSeverity.Warning, true),
            new DiagnosticDescriptor(DiagnosticCodes.AssembliesHaveCopyrightAttributeAnalyzer, "Assemblies should have a copyright attribute", "Assemblies should have a copyright attribute", "Design", DiagnosticSeverity.Warning, true),
            new DiagnosticDescriptor(DiagnosticCodes.AssembliesHaveDescriptionAttributeAnalyzer, "Assemblies should have a description attribute", "Assemblies should have a description attribute", "Design", DiagnosticSeverity.Warning, true),
            new DiagnosticDescriptor(DiagnosticCodes.AssembliesHaveTitleAttributeAnalyzer, "Assemblies should have a title attribute", "Assemblies should have a title attribute", "Design", DiagnosticSeverity.Warning, true)
        );

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            // Get the particular attributes I need to look for.
            var companyAttributeSymbol = KnownTypes.CompanyAttribute(context.Compilation);
            var copyrightAttributeSymbol = KnownTypes.CopyrightAttribute(context.Compilation);
            var descriptionAttributeSymbol = KnownTypes.DescriptionAttribute(context.Compilation);
            var titleAttributeSymbol = KnownTypes.TitleAttribute(context.Compilation);

            var symbolEqualityComparer = SymbolEqualityComparer.Default;

            // Assume they are all not found.
            Boolean companyAttributeGood = false;
            Boolean copyrightAttributeGood = false;
            Boolean descriptionAttributeGood = false;
            Boolean titleAttributeGood = false;

            // Pound through each attribute in the assembly checking that the specific ones
            // are present and the parameters are not empty.
            foreach (var attribute in context.Compilation.Assembly.GetAttributes())
            {
                if ((companyAttributeSymbol != null) && (attribute.AttributeClass != null) && symbolEqualityComparer.Equals(attribute.AttributeClass, companyAttributeSymbol))
                {
                    companyAttributeGood = CheckAttributeParameter(attribute);
                    continue;
                }

                if ((copyrightAttributeSymbol != null) && (attribute.AttributeClass != null) && symbolEqualityComparer.Equals(attribute.AttributeClass, copyrightAttributeSymbol))
                {
                    copyrightAttributeGood = CheckAttributeParameter(attribute);
                    continue;
                }

                if ((descriptionAttributeSymbol != null) && (attribute.AttributeClass != null) && symbolEqualityComparer.Equals(attribute.AttributeClass, descriptionAttributeSymbol))
                {
                    descriptionAttributeGood = CheckAttributeParameter(attribute);
                    continue;
                }

                if ((titleAttributeSymbol != null) && (attribute.AttributeClass != null) && symbolEqualityComparer.Equals(attribute.AttributeClass, titleAttributeSymbol))
                {
                    titleAttributeGood = CheckAttributeParameter(attribute);
                    continue;
                }
            }

            // If any of the assembly wide attributes are missing or empty, trigger a warning.
            if (!companyAttributeGood)
            {
                context.ReportDiagnostic(Diagnostic.Create(SupportedDiagnostics.Select(x => x).First(x => x.Id == DiagnosticCodes.AssembliesHaveCompanyAttributeAnalyzer), Location.None));
            }

            if (!copyrightAttributeGood)
            {
                context.ReportDiagnostic(Diagnostic.Create(SupportedDiagnostics.Select(x => x).First(x => x.Id == DiagnosticCodes.AssembliesHaveCopyrightAttributeAnalyzer), Location.None));
            }

            if (!descriptionAttributeGood)
            {
                context.ReportDiagnostic(Diagnostic.Create(SupportedDiagnostics.Select(x => x).First(x => x.Id == DiagnosticCodes.AssembliesHaveDescriptionAttributeAnalyzer), Location.None));
            }

            if (!titleAttributeGood)
            {
                context.ReportDiagnostic(Diagnostic.Create(SupportedDiagnostics.Select(x => x).First(x => x.Id == DiagnosticCodes.AssembliesHaveTitleAttributeAnalyzer), Location.None));
            }
        }

        private static Boolean CheckAttributeParameter(AttributeData attribute)
        {
            if (attribute.ConstructorArguments.Length == 1) 
            {
                String? param = attribute.ConstructorArguments[0].Value?.ToString();
                if (!String.IsNullOrEmpty(param))
                {
                    return true;
                }
            }

            return false;
        }

        private static class KnownTypes
        {
            public static INamedTypeSymbol CompanyAttribute(Compilation compilation)
            {
                return compilation.GetTypeByMetadataName("System.Reflection.AssemblyCompanyAttribute") ?? throw new InvalidOperationException("Type not found: System.Reflection.AssemblyCompanyAttribute");
            }

            public static INamedTypeSymbol CopyrightAttribute(Compilation compilation)
            {
                return compilation.GetTypeByMetadataName("System.Reflection.AssemblyCopyrightAttribute") ?? throw new InvalidOperationException("Type not found: System.Reflection.AssemblyCopyrightAttribute");
            }

            public static INamedTypeSymbol DescriptionAttribute(Compilation compilation)
            {
                return compilation.GetTypeByMetadataName("System.Reflection.AssemblyDescriptionAttribute") ?? throw new InvalidOperationException("Type not found: System.Reflection.AssemblyDescriptionAttribute");
            }

            public static INamedTypeSymbol TitleAttribute(Compilation compilation)
            {
                return compilation.GetTypeByMetadataName("System.Reflection.AssemblyTitleAttribute") ?? throw new InvalidOperationException("Type not found: System.Reflection.AssemblyTitleAttribute");
            }
        }
    }
}