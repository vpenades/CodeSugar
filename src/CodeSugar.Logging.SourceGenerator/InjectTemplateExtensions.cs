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
            ProcessTemplates(context,"Progress", n => n.Contains(".Progress."));            
        }

        private int _TemplateIndex = 0;

        private void ProcessTemplates(SourceProductionContext context, string name, Predicate<string> nameChecker)
        {
            var processor = new TemplateCodeProcessor();
            processor.RootNameSpace = this.RootNameSpace;
            processor.AllNugets = this.NugetPackages;

            foreach (var code in EmbeddedTemplates.GetEmbeddedTemplates(name, nameChecker))
            {
                var xcode = processor.ProcessTemplate(code);

                context.AddSource($"CodeSugar.{name}{_TemplateIndex}.g.cs", xcode);
                _TemplateIndex++;
            }
        }
    }
}
