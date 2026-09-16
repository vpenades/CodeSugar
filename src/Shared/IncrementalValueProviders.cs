using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeSugar
{
    static class IncrementalValueProviders
    {
        #region untangler

        public static (T0, T1, T2, T3, T4) Untangle<T0, T1, T2, T3, T4>(((((T0, T1), T2), T3), T4) tangled)
        {
            var (item0123, item4) = tangled;
            var (item012, item3) = item0123;
            var (item01, item2) = item012;
            var (item0, item1) = item01;

            return (item0, item1, item2, item3, item4);
        }

        public static (T0, T1, T2, T3) Untangle<T0, T1, T2, T3>((((T0, T1), T2), T3) tangled)
        {
            var (item012, item3) = tangled;
            var (item01, item2) = item012;
            var (item0, item1) = item01;

            return (item0, item1, item2, item3);
        }

        public static (T0, T1, T2) Untangle<T0, T1, T2>(((T0, T1), T2) tangled)
        {
            var (item01, item2) = tangled;
            var (item0, item1) = item01;

            return (item0, item1, item2);
        }

        #endregion

        #region root namespace

        public static IncrementalValueProvider<string?> GetRootNamespaceProvider(this IncrementalGeneratorInitializationContext context)
        {
            return context.AnalyzerConfigOptionsProvider.Select(TryGetRootNamespace);
        }

        private static string? TryGetRootNamespace(AnalyzerConfigOptionsProvider options, CancellationToken token)
        {
            // retrieve Root namespace. MSBuild properties require the "build_property." prefix
            return options.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace)
                ? rootNamespace
                : null;
        }

        #endregion

        #region LanguageVersion

        public static IncrementalValueProvider<LanguageVersion> GetLanguageVersionProvider(this IncrementalGeneratorInitializationContext context)
        {
            return context.ParseOptionsProvider.Select(TryGetLanguageFeatures);
        }

        private static LanguageVersion TryGetLanguageFeatures(ParseOptions options, CancellationToken token)
        {
            if (options is not CSharpParseOptions csParseOptions)
            {
                throw new NotSupportedException($"Only {LanguageNames.CSharp} is supported.");
            }

            var langVersion = csParseOptions.LanguageVersion.MapSpecifiedToEffectiveVersion();

            if (langVersion == LanguageVersion.Default) throw new InvalidOperationException("invalid language version");

            return langVersion;
        }

        #endregion

        #region Nuget Package References

        public static IncrementalValueProvider<Dictionary<string, string>> GetPackageReferencesProvider(this IncrementalGeneratorInitializationContext context)
        {
            return context.CompilationProvider.Select(TryGetNugetPackages);
        }

        private static Dictionary<string, string> TryGetNugetPackages(Compilation compilation, CancellationToken token)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var reference in compilation.References)
            {
                if (reference is not PortableExecutableReference peRef) continue;
                if (compilation.GetAssemblyOrModuleSymbol(peRef) is not IAssemblySymbol symbol) continue;

                string version = string.Empty;

                // Check InternalsVisibleTo or other assembly attributes
                foreach (var attr in symbol.GetAttributes())
                {
                    if (attr.AttributeClass?.Name == "InformationalVersionAttribute" ||
                        attr.AttributeClass?.Name == "AssemblyVersionAttribute")
                    {
                        version = attr.ToString();
                    }
                }

                dict.Add(symbol.Name, version);
            }

            return dict;
        }

        #endregion

        #region specific types

        private static Dictionary<string, bool>? TryGetSpecificTypes(Compilation compilation, CancellationToken token, params string[] typeFullNames)
        {
            var dict = new Dictionary<string, bool>();

            foreach (var tfn in typeFullNames)
            {
                dict[tfn] = compilation.GetTypeByMetadataName(tfn) != null;
            }

            return dict;
        }

        #endregion
    }
}