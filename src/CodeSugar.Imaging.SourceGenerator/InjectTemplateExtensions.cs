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
            var hasTensors = this.NugetPackages.ContainsKey("System.Numerics.Tensors");
            var hasImageSharp = this.NugetPackages.ContainsKey("SixLabors.ImageSharp");
            var hasMagicScaler = this.NugetPackages.ContainsKey("PhotoSauce.MagicScaler");
            var hasSkiaSharp = this.NugetPackages.ContainsKey("SkiaSharp");
            var hasAvalonia = this.NugetPackages.ContainsKey("Avalonia");
            var hasTensorBitmaps = this.NugetPackages.ContainsKey("InteropTypes.TensorBitmaps.Core");

            ProcessTemplates(context,"Core", n => n.Contains(".Templates.Core."));

            // imaging

            if (hasImageSharp) ProcessTemplates(context, "ImageSharp", n => n.Contains(".Templates.ImageSharp."));
            if (hasMagicScaler) ProcessTemplates(context, "MagicScaler", n => n.Contains(".Templates.MagicScaler."));
            if (hasSkiaSharp) ProcessTemplates(context, "SkiaSharp", n => n.Contains(".Templates.SkiaSharp."));
            if (hasAvalonia) ProcessTemplates(context, "Avalonia", n => n.Contains(".Templates.Avalonia."));            

            if (hasTensors) // tensors
            {
                ProcessTemplates(context, "Tensors", n => n.Contains(".Templates.Tensors."));
                ProcessTemplates(context, "Intrinsics", n => n.Contains(".Templates.Intrinsics."));
            }            

            if (hasTensors) // imaging interop with tensors
            {
                if (hasImageSharp) ProcessTemplates(context, "ImageSharpTensors", n => n.Contains(".Templates.ImageSharpTensors."));
                if (hasMagicScaler) ProcessTemplates(context, "MagicScalerTensors", n => n.Contains(".Templates.MagicScalerTensors."));
                if (hasSkiaSharp) ProcessTemplates(context, "SkiaSharpTensors", n => n.Contains(".Templates.SkiaSharpTensors."));
                if (hasTensorBitmaps) ProcessTemplates(context, "InteropTensorBitmaps", n => n.Contains(".Templates.InteropTensorBitmaps."));
            }            
        }

        private int _TemplateIndex = 0;

        private void ProcessTemplates(SourceProductionContext context, string name, Predicate<string> nameChecker)
        {
            var processor = new TemplateCodeProcessor();
            processor.RootNameSpace = this.RootNameSpace;
            processor.AllNugets = this.NugetPackages;
            processor.UsesNuget("System.Numerics.Tensors");
            processor.UsesNuget("SixLabors.ImageSharp");
            processor.UsesNuget("PhotoSauce.MagicScaler");
            processor.UsesNuget("SkiaSharp");
            processor.UsesNuget("Avalonia");
            processor.UsesNuget("InteropTypes.TensorBitmaps.Core");

            foreach (var code in EmbeddedTemplates.GetEmbeddedTemplates(name, nameChecker))
            {
                var xcode = processor.ProcessTemplate(code);

                context.AddSource($"CodeSugar.{name}{_TemplateIndex}.g.cs", xcode);
                _TemplateIndex++;
            }
        }
    }
}
