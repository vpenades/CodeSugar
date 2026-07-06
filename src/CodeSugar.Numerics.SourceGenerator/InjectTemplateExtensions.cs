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
            var hasOnnx = this.NugetPackages.ContainsKey("Microsoft.ML.OnnxRuntime"); // although we include Microsoft.ML.OnnxRuntime.Managed, this is what shows up
            var hasNCalc6 = this.NugetPackages.ContainsKey("NCalc"); // check version is 6 or higher

            ProcessTemplates(context,"Vectors", n => n.Contains(".Templates.Vectors."));
            ProcessTemplates(context, "Matrices", n => n.Contains(".Templates.Matrices."));

            if (hasTensors) ProcessTemplates(context,"Tensors", n => n.Contains(".Templates.Tensors."));
            if (hasOnnx) ProcessTemplates(context, "OnnxRuntime", n => n.Contains(".Templates.OnnxRuntime."));
            if (hasNCalc6) ProcessTemplates(context, "NCalc", n => n.Contains(".Templates.NCalc."));            
        }

        private int _TemplateIndex = 0;

        private void ProcessTemplates(SourceProductionContext context, string name, Predicate<string> nameChecker)
        {
            var processor = new TemplateCodeProcessor();
            processor.AllNugets = this.NugetPackages;
            processor.RootNameSpace = this.RootNameSpace;
            processor.UsesNuget("System.Numerics.Tensors");
            processor.UsesNuget("Microsoft.ML.OnnxRuntime");
            processor.UsesNuget("MathNet.Numerics");
            processor.UsesNuget("NCalc");
            

            foreach (var code in EmbeddedTemplates.GetEmbeddedTemplates(name, nameChecker))
            {
                var xcode = processor.ProcessTemplate(code);

                context.AddSource($"CodeSugar.{name}{_TemplateIndex}.g.cs", xcode);
                _TemplateIndex++;
            }
        }
    }
}
