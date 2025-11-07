using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace CodeSugar
{
    internal static class _RoslynExtensions
    {
        public static bool CheckUsesProperty<T>(string sourceCode, string propertyName)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceCode);

            var compilation = CSharpCompilation.Create("TestAssembly",
                new[] { tree },
                new[] {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(T).Assembly.Location)
                });

            var model = compilation.GetSemanticModel(tree);

            var root = tree.GetRoot();
            var memberAccesses = root.DescendantNodes().OfType<MemberAccessExpressionSyntax>();

            return memberAccesses.Any(m => _CheckProperty<T>(model, m, propertyName));
        }

        private static bool _CheckProperty<T>(SemanticModel model, MemberAccessExpressionSyntax m, string propertyName)
        {
            // Check if property name is the one we're looking for
            if (m.Name.Identifier.Text != propertyName) return false;

            // Get the type of the expression before '.PropertyName'

            if (model.GetSymbolInfo(m).Symbol is not IPropertySymbol symbol) return false;

            var result = symbol != null &&
                       symbol.Name == propertyName &&
                       symbol.ContainingType.ToString() == typeof(T).FullName;


            if (new PragmaContext(m).IsInsidePragmaWarningDisable(m)) return false;

            return result;
        }

        public static IEnumerable<KeyValuePair<string,string>> EnumerateUsingAliasDirectives(string sourceCode)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = syntaxTree.GetRoot();

            var usingDirectives = root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Where(u => u.Alias != null);            

            foreach (var usingDirective in usingDirectives)
            {
                // Alias name
                var alias = usingDirective.Alias.Name.Identifier.Text;
                // The fully-qualified name/expression
                var name = usingDirective.Name.ToString();

                yield return new KeyValuePair<string, string>(alias, name);
            }
        }
    }

    internal class PragmaContext
    {
        public PragmaContext(MemberAccessExpressionSyntax memberAccess) : this(memberAccess.SyntaxTree.GetRoot()) { }
        public PragmaContext(SyntaxNode root)
        {
            _Root = root;

            var directives = root
                .DescendantNodesAndTokens(descendIntoTrivia: true)
                .Where(n => n.IsNode && n.AsNode() is DirectiveTriviaSyntax)
                .Select(n => (DirectiveTriviaSyntax)n.AsNode())
                .ToList();
            
            _Disables = directives
                .Where(d => d.IsKind(SyntaxKind.PragmaWarningDirectiveTrivia) &&
                            ((PragmaWarningDirectiveTriviaSyntax)d).DisableOrRestoreKeyword.IsKind(SyntaxKind.DisableKeyword))
                .Cast<PragmaWarningDirectiveTriviaSyntax>()
                .ToList();

            _Restores = directives
                .Where(d => d.IsKind(SyntaxKind.PragmaWarningDirectiveTrivia) &&
                            ((PragmaWarningDirectiveTriviaSyntax)d).DisableOrRestoreKeyword.IsKind(SyntaxKind.RestoreKeyword))
                .Cast<PragmaWarningDirectiveTriviaSyntax>()
                .ToList();
        }

        private readonly SyntaxNode _Root;
        private readonly IReadOnlyList<PragmaWarningDirectiveTriviaSyntax> _Disables;
        private readonly IReadOnlyList<PragmaWarningDirectiveTriviaSyntax> _Restores;

        public bool IsInsidePragmaWarningDisable(MemberAccessExpressionSyntax memberAccess)
        {
            var span = memberAccess.Span.Start;

            foreach (var disable in _Disables)
            {
                var disablePos = disable.FullSpan.End;
                var restore = _Restores.FirstOrDefault(r => r.FullSpan.Start > disable.FullSpan.End);
                var restorePos = restore?.FullSpan.Start ?? _Root.FullSpan.End;

                if (span >= disablePos && span < restorePos)
                    return true;
            }

            return false;
        }
    }
}
