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
    
    public abstract class CodeInjectorGenerator : IIncrementalGenerator
    {
        // https://mstack.nl/blogs/source-generators-incremental/

        #region lifecycle
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // triggers | todo: get frameworks  
            var languageDataProvider = context.ParseOptionsProvider.Select(TryGetLanguageFeatures);    // trigger when language changes
            var withRootNamespace = context.AnalyzerConfigOptionsProvider.Select(TryGetRootNamespace); // trigger when RootNamespace changes
            var withNugetPackages = context.CompilationProvider.Select(TryGetNugetPackages);           // trigger when package references changes            

            // combined triggers
            var provider = withRootNamespace
                .Combine(languageDataProvider)
                .Combine(withNugetPackages);

            // executed on any triggers signal
            context.RegisterSourceOutput(provider, (ctx, args) => _TryInjectSources(ctx, args.Left.Left, args.Left.Right, args.Right) );
        }

        private static LanguageVersion TryGetLanguageFeatures(ParseOptions options, CancellationToken token)
        {
            if (options is not CSharpParseOptions csParseOptions)
            {
                throw new NotSupportedException($"Only {LanguageNames.CSharp} is supported.");
            }            

            return csParseOptions.LanguageVersion;
        }

        private static string? TryGetRootNamespace(AnalyzerConfigOptionsProvider options, CancellationToken token)
        {
            // retrieve Root namespace. MSBuild properties require the "build_property." prefix
            return options.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace)
                ? rootNamespace
                : null;
        }

        private static Dictionary<string,string>? TryGetNugetPackages(Compilation compilation, CancellationToken token)
        {
            var dict = new Dictionary<string, string>();            

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

        #region API

        private void _TryInjectSources(SourceProductionContext context, string rootNamespace, LanguageVersion lang, Dictionary<string,string>? nupkgs)
        {
            var ns = rootNamespace?.Trim();

            if (string.IsNullOrWhiteSpace(ns))
            {                
                var ddd = new DiagnosticDescriptor("CSUGAR1000", "RootNamespace required.", "Define a <RootNamespace>namespace</RootNamespace> in the project.", "CodeSugar", DiagnosticSeverity.Error, true);
                context.ReportDiagnostic(Diagnostic.Create(ddd, null));
                return;
            }

            if (nupkgs == null) return;

            RootNameSpace = ns!;
            NugetPackages = nupkgs;
            LangVersion = lang;

            InjectSources(context);
        }

        protected LanguageVersion LangVersion { get; private set; }

        protected bool LangSupportsNullable => LangVersion >= LanguageVersion.CSharp8;
        protected bool LangSupportsGenericAttrs => LangVersion >= LanguageVersion.CSharp11;        

        protected string RootNameSpace { get; private set; } = string.Empty;

        protected IReadOnlyDictionary<string,string> NugetPackages { get; private set; } = new Dictionary<string,string>();

        protected abstract void InjectSources(SourceProductionContext context);

        #endregion

        #region diagnostics

        protected static readonly DiagnosticDescriptor LogInfo1 = new DiagnosticDescriptor(
            id: "LOG001",
            title: "Generator Log",
            messageFormat: "Log del generador: {0}",
            category: "SourceGenerator",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true);

        #endregion
    }


    record SemanticVersion : IComparable<SemanticVersion>
    {
        public static bool TryParse(string fullVersion, out SemanticVersion sv)
        {
            if (string.IsNullOrEmpty(fullVersion))
            {
                sv = Default100;
                return false;
            }

            var suffix = string.Empty;

            var sidx = fullVersion.IndexOf('-');
            if (sidx >= 0)
            {
                suffix = fullVersion.Substring(sidx + 1);
                fullVersion = fullVersion.Substring(0, sidx);
            }

            if (!Version.TryParse(fullVersion, out var version))
            {
                sv = Default100;
                return false;
            }

            sv = new SemanticVersion(version, suffix);
            return true;
        }

        private static readonly SemanticVersion Default100 = new SemanticVersion(new Version(1, 0, 0), string.Empty);

        private SemanticVersion(Version version, string suffix)
        {
            Version = version;
            Suffix = suffix;
        }

        public Version Version { get; } = new Version(1, 0, 0);

        public string Suffix { get; } = string.Empty;

        public int CompareTo(SemanticVersion other)
        {
            var r = this.Version.CompareTo(other.Version);
            if (r != 0) return r;

            var thisHasPrefix = !string.IsNullOrEmpty(this.Suffix);
            var otherHasPrefix = !string.IsNullOrEmpty(other.Suffix);
            if (!thisHasPrefix && !otherHasPrefix) return 0;

            r = thisHasPrefix.CompareTo(otherHasPrefix);
            if (r != 0) return r;

            // suffix rule comparison are tricky, but generally we're going to work with full versions

            return this.Suffix.CompareTo(other.Suffix, StringComparison.OrdinalIgnoreCase);
        }
    }

}
