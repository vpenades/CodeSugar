using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

namespace CodeSugar
{
    [Generator]
    public sealed class InjectTemplateExtensions : CodeInjectorGenerator
    {
        protected override void InjectSources(SourceProductionContext context)
        {
            ProcessTemplates(context,"Binary", n => n.Contains(".Binary."));            
        }

        private int _TemplateIndex = 0;

        private void ProcessTemplates(SourceProductionContext context, string name, Predicate<string> nameChecker)
        {
            var processor = new TemplateCodeProcessor(this.RootNameSpace, this.LangVersion, this.NugetPackages);

            foreach (var code in EmbeddedTemplates.GetEmbeddedTemplates(name, nameChecker))
            {
                var xcode = processor.ProcessTemplate(code);

                context.AddSource($"CodeSugar.{name}{_TemplateIndex}.g.cs", xcode);
                _TemplateIndex++;
            }
        }
    }
}
