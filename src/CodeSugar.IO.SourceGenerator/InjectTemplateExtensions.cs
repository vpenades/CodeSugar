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
            var hasAbstractions = this.NugetPackages.ContainsKey("Microsoft.Extensions.FileProviders.Abstractions");                        
            var hasSharpCompress = this.NugetPackages.ContainsKey("SharpCompress");            

            ProcessTemplates(context,"SystemIO", n => n.Contains(".Templates.SystemIO."));

            if (hasAbstractions) ProcessTemplates(context,"FileProviders", n => n.Contains(".Templates.FileProviders."));
            if (hasSharpCompress) ProcessTemplates(context, "SharpCompress", n => n.Contains(".Templates.SharpCompress."));
        }

        private int _TemplateIndex = 0;

        private void ProcessTemplates(SourceProductionContext context, string name, Predicate<string> nameChecker)
        {
            var processor = new TemplateCodeProcessor();
            processor.RootNameSpace = this.RootNameSpace;
            processor.AllNugets = this.NugetPackages;
            processor.UsesNuget("Microsoft.Extensions.FileProviders.Abstractions");
            processor.UsesNuget("Microsoft.Extensions.FileProviders.Physical");
            processor.UsesNuget("Microsoft.Extensions.FileProviders.Embedded");
            processor.UsesNuget("Microsoft.Extensions.FileProviders.Composite");
            processor.UsesNuget("RecyclableMemoryStream");
            processor.UsesNuget("SharpCompress");

            foreach (var code in EmbeddedTemplates.GetEmbeddedTemplates(name, nameChecker))
            {
                var xcode = processor.ProcessTemplate(code);

                context.AddSource($"CodeSugar.{name}{_TemplateIndex}.g.cs", xcode);
                _TemplateIndex++;
            }            
        }
    }
}
