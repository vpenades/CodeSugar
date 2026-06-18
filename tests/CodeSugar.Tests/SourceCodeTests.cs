using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodeSugar
{
    internal class SourceCodeTests
    {
        [Test]
        [Explicit] // to be replaced by a template analyzer
        [Arguments(typeof(CodeSugarForSystem))]        
        [Arguments(typeof(CodeSugarForSerialization))]        
        
        [Arguments(typeof(CodeSugarForLogging))]

        [Arguments(typeof(CodeSugarForImageSharp))]

        [Arguments(typeof(CodeSugarForLinq))]
        public void ListApiMethods(Type t)
        {
            var methods = ApiInfo.ListMethods(t.Assembly).ToList();

            var groups = methods.GroupBy(item => item.ClassType);

            foreach (var group in groups)
            {
                Console.Out.WriteLine($"--------- {group.Key.Name}");

                var maxReturnLen = methods.Max(item => item.ToReturnString().Length);

                foreach (var method in methods.OrderBy(item => item.ToBodyString()))
                {
                    var retName = method.ToReturnString();
                    while (retName.Length < maxReturnLen) retName += " ";

                    Console.Out.WriteLine($"{retName} {method.ToBodyString()}");
                }

                Console.Out.WriteLine(string.Empty);
            }
        }


        [Test]
        [Explicit] // to be replaced by a template analyzer
        [Arguments("CodeSugar.Sys.IO.Sources")]
        [Arguments("CodeSugar.Sys.Sources")]
        [Arguments("CodeSugar.Srlzn.Bin.Sources")]
        [Arguments("CodeSugar.FileProviders.Sources")]        
        public async Task TestUsingExistsProperty(string projectName)
        {
            var dinfo = new System.IO.DirectoryInfo(AppContext.BaseDirectory).FindDirectoryTree("src", projectName);
            await Assert.That(dinfo.Exists).IsTrue();

            foreach(var finfo in dinfo.EnumerateFiles("*.cs", System.IO.SearchOption.TopDirectoryOnly))
            {
                var sc = finfo.ReadAllText();

                var result1 = _RoslynExtensions.CheckUsesProperty<System.IO.FileInfo>(sc, "Exists");
                await Assert.That(result1).IsFalse().Because($"{finfo.Name} uses System.IO.FileInfo.Exists");

                var result2 = _RoslynExtensions.CheckUsesProperty<System.IO.FileInfo>(sc, "Length");
                await Assert.That(result2).IsFalse().Because($"{finfo.Name} uses System.IO.FileInfo.Length");

                var result3 = _RoslynExtensions.CheckUsesProperty<System.IO.DirectoryInfo>(sc, "Exists");
                await Assert.That(result3).IsFalse().Because($"{finfo.Name} uses System.IO.DirectoryInfo.Exists");
            }            
        }

        /// <summary>
        /// tests that all files have #nullable disable
        /// </summary>
        /// <param name="projectName"></param>

        [Test]
        [Explicit] // to be replaced by a template analyzer
        [Arguments("CodeSugar.Sys.Sources")]
        [Arguments("CodeSugar.Sys.IO.Sources")]        
        [Arguments("CodeSugar.Sys.Text.Sources")]
        [Arguments("CodeSugar.Srlzn.Bin.Sources")]
        [Arguments("CodeSugar.FileProviders.Sources")]
        

        [Arguments("CodeSugar.Linq.Sources")]
        [Arguments("CodeSugar.Numerics.Sources")]
        [Arguments("CodeSugar.Tensors.Sources")]

        [Arguments("CodeSugar.Progress.Log")]
        [Arguments("CodeSugar.AI")]
        [Arguments("CodeSugar.ImageSharp")]

        public async Task TestNullableDisabled(string projectName)
        {
            var dinfo = new System.IO.DirectoryInfo(AppContext.BaseDirectory).FindDirectoryTree("src", projectName);
            await Assert.That(dinfo.Exists).IsTrue();

            foreach (var finfo in dinfo.EnumerateFiles("*.cs", System.IO.SearchOption.TopDirectoryOnly))
            {
                var sc = finfo.ReadAllText();
                // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.

                await Assert.That(sc).Contains("#nullable disable").Because($"{projectName}/{finfo.Name} does not have #nullable disable");
            }
        }


        /// <summary>
        /// Tests whether all using xxx = yyy; instances begin with double underscores '__' to prevent collisions with global usings.
        /// </summary>
        /// <param name="projectName"></param>
        
        [Test]
        [Explicit] // to be replaced by a template analyzer
        [Arguments("CodeSugar.Sys.Sources")]
        [Arguments("CodeSugar.Sys.IO.Sources")]
        [Arguments("CodeSugar.Sys.Text.Sources")]
        [Arguments("CodeSugar.Srlzn.Bin.Sources")]
        [Arguments("CodeSugar.FileProviders.Sources")]        

        [Arguments("CodeSugar.Linq.Sources")]
        [Arguments("CodeSugar.Numerics.Sources")]
        [Arguments("CodeSugar.Tensors.Sources")]        

        [Arguments("CodeSugar.Progress.Log")]
        [Arguments("CodeSugar.AI")]
        [Arguments("CodeSugar.ImageSharp")]        
        public async Task TestUsingAlias(string projectName)
        {
            var dinfo = new System.IO.DirectoryInfo(AppContext.BaseDirectory).FindDirectoryTree("src", projectName);
            await Assert.That(dinfo.Exists).IsTrue();

            using var scope = Assert.Multiple();

            foreach (var finfo in dinfo.EnumerateFiles("*.cs", System.IO.SearchOption.TopDirectoryOnly))
            {
                var sc = finfo.ReadAllText();

                foreach (var kvp in _RoslynExtensions.EnumerateUsingAliasDirectives(sc).Distinct())
                {
                    var alias = kvp.Key;
                    // TODO: TUnit migration - Complex NUnit constraint. Manual conversion required.

                    await Assert.That(alias).StartsWith("__").Because($"using {alias} in file {finfo.FullName} must begin with '__' to prevent collisions with global using");
                }
            }
        }
    }
}