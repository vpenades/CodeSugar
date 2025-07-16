using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

using NUnit.Framework;

namespace CodeSugar
{
    internal class SourceCodeTests
    {
        [TestCase(typeof(CodeSugarForSystem))]
        [TestCase(typeof(CodeSugarForSystemIO))]
        [TestCase(typeof(CodeSugarForFileProviders))]
        [TestCase(typeof(CodeSugarForSerialization))]

        [TestCase(typeof(CodeSugarForNumerics))]
        [TestCase(typeof(CodeSugarForTensors))]
        
        [TestCase(typeof(CodeSugarForLogging))]

        [TestCase(typeof(CodeSugarForLinq))]
        public void ListApiMethods(Type t)
        {
            var methods = ApiInfo.ListMethods(t.Assembly).ToList();

            var groups = methods.GroupBy(item => item.ClassType);

            foreach (var group in groups)
            {
                TestContext.Out.WriteLine($"--------- {group.Key.Name}");

                var maxReturnLen = methods.Max(item => item.ToReturnString().Length);

                foreach (var method in methods.OrderBy(item => item.ToBodyString()))
                {
                    var retName = method.ToReturnString();
                    while (retName.Length < maxReturnLen) retName += " ";

                    TestContext.Out.WriteLine($"{retName} {method.ToBodyString()}");
                }

                TestContext.Out.WriteLine(string.Empty);
            }
        }



        [TestCase("CodeSugar.Sys.IO.Sources")]
        [TestCase("CodeSugar.Sys.Sources")]
        [TestCase("CodeSugar.Srlzn.Bin.Sources")]
        [TestCase("CodeSugar.FileProviders.Sources")]        
        public void TestUsingExistsProperty(string projectName)
        {
            var dinfo = new System.IO.DirectoryInfo(TestContext.CurrentContext.TestDirectory).FindDirectoryTree("src", projectName);
            Assert.That(dinfo.Exists);

            foreach(var finfo in dinfo.EnumerateFiles("*.cs", System.IO.SearchOption.TopDirectoryOnly))
            {
                var sc = finfo.ReadAllText();

                var result1 = _RoslynExtensions.CheckUsesProperty<System.IO.FileInfo>(sc, "Exists");
                Assert.That(result1, Is.False, $"{finfo.Name} uses System.IO.FileInfo.Exists");

                var result2 = _RoslynExtensions.CheckUsesProperty<System.IO.FileInfo>(sc, "Length");
                Assert.That(result2, Is.False, $"{finfo.Name} uses System.IO.FileInfo.Length");

                var result3 = _RoslynExtensions.CheckUsesProperty<System.IO.DirectoryInfo>(sc, "Exists");
                Assert.That(result3, Is.False, $"{finfo.Name} uses System.IO.DirectoryInfo.Exists");
            }            
        }

        [TestCase("CodeSugar.Sys.Sources")]
        [TestCase("CodeSugar.Sys.IO.Sources")]        
        [TestCase("CodeSugar.Sys.Text.Sources")]
        [TestCase("CodeSugar.Srlzn.Bin.Sources")]
        [TestCase("CodeSugar.FileProviders.Sources")]

        [TestCase("CodeSugar.Linq.Sources")]
        [TestCase("CodeSugar.Numerics.Sources")]
        [TestCase("CodeSugar.Tensors.Sources")]
        [TestCase("CodeSugar.Progress.Log")]
        
        public void TestNullableDisabled(string projectName)
        {
            var dinfo = new System.IO.DirectoryInfo(TestContext.CurrentContext.TestDirectory).FindDirectoryTree("src", projectName);
            Assert.That(dinfo.Exists);

            foreach (var finfo in dinfo.EnumerateFiles("*.cs", System.IO.SearchOption.TopDirectoryOnly))
            {
                var sc = finfo.ReadAllText();

                Assert.That(sc.Contains("#nullable disable"), $"{projectName}/{finfo.Name} does not have #nullable disable");
            }

        }
    }
}
