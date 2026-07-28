using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeSugar
{
    [Generator]
    public sealed class FileIOGenerator : CodeInjectorGenerator
    {
        protected override void InjectSources(SourceProductionContext context, CodeGenerationContext cgc)
        {
            var hasAbstractions = cgc.NugetPackages.ContainsKey("Microsoft.Extensions.FileProviders.Abstractions");                        
            var hasSharpCompress = cgc.NugetPackages.ContainsKey("SharpCompress");
            var hasMonoAndroid = cgc.NugetPackages.ContainsKey("MonoAndroid");

            ProcessTemplates(context, cgc, "SystemIO", n => n.Contains(".Templates.SystemIO."));

            if (hasAbstractions) ProcessTemplates(context, cgc, "FileProviders", n => n.Contains(".Templates.FileProviders."));
            if (hasSharpCompress) ProcessTemplates(context, cgc,"SharpCompress", n => n.Contains(".Templates.SharpCompress."));
        }

        private int _TemplateIndex = 0;

        private void ProcessTemplates(SourceProductionContext context, CodeGenerationContext cgc, string name, Predicate<string> nameChecker)
        {
            var processor = new TemplateCodeProcessor(cgc);

            processor.UsesNuget("System.Text.Json");
            processor.UsesNuget("Mono.Android");
            processor.UsesNuget("Microsoft.Extensions.FileProviders.Abstractions");
            processor.UsesNuget("Microsoft.Extensions.FileProviders.Physical");
            processor.UsesNuget("Microsoft.Extensions.FileProviders.Embedded");
            processor.UsesNuget("Microsoft.Extensions.FileProviders.Composite");
            processor.UsesNuget("Microsoft.IO.RecyclableMemoryStream");
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
