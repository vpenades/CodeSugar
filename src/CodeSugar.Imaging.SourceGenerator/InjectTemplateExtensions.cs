using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

namespace CodeSugar
{
    [Generator]
    public sealed class CodeSugarImagingGenerator : CodeInjectorGenerator
    {
        protected override void InjectSources(SourceProductionContext context, CodeGenerationContext cgc)
        {
            var hasTensors = cgc.NugetPackages.ContainsKey("System.Numerics.Tensors");            

            ProcessTemplates(context,cgc, "Core", n => n.Contains(".Templates.Core."));            

            if (hasTensors) // tensors
            {
                ProcessTemplates(context, cgc, "Tensors", n => n.Contains(".Templates.Tensors."));
                ProcessTemplates(context, cgc, "Intrinsics", n => n.Contains(".Templates.Intrinsics."));
            }

            // imaging

            ProcessTemplates(context, cgc, "ImageSharp", n => n.Contains(".Templates.ImageSharp."));
            ProcessTemplates(context, cgc, "MagicScaler", n => n.Contains(".Templates.MagicScaler."));
            ProcessTemplates(context, cgc, "SkiaSharp", n => n.Contains(".Templates.SkiaSharp."));
            ProcessTemplates(context, cgc, "Avalonia", n => n.Contains(".Templates.Avalonia."));
            ProcessTemplates(context, cgc, "InteropTensorBitmaps", n => n.Contains(".Templates.InteropTensorBitmaps."));                     
        }

        private int _TemplateIndex = 0;

        private void ProcessTemplates(SourceProductionContext context, CodeGenerationContext cgc, string name, Predicate<string> nameChecker)
        {
            var processor = new TemplateCodeProcessor(cgc);            
            
            processor.UsesNuget("System.Numerics.Tensors");
            processor.UsesNuget("SixLabors.ImageSharp");
            processor.UsesNuget("PhotoSauce.MagicScaler");
            processor.UsesNuget("SkiaSharp");
            processor.UsesNuget("Avalonia");
            processor.UsesNuget("InteropTypes.TensorBitmaps.Core");

            foreach (var code in EmbeddedTemplates.GetEmbeddedTemplates(name, nameChecker))
            {
                if (!cgc.CheckCodeRequirements(code)) continue;

                var xcode = processor.ProcessTemplate(code);

                context.AddSource($"CodeSugar.{name}{_TemplateIndex}.g.cs", xcode);
                _TemplateIndex++;
            }
        }
    }
}
