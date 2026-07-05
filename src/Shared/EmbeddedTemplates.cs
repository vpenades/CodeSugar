using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

namespace CodeSugar
{
    static class EmbeddedTemplates
    {
        public static string[] GetEmbeddedTemplates(string name, Predicate<string> nameChecker)
        {
            var selfAssembly = typeof(EmbeddedTemplates).Assembly;

            var files = new List<string>();

            foreach (var resName in selfAssembly.GetManifestResourceNames())
            {
                if (!resName.EndsWith(".cs")) continue;

                if (!nameChecker(resName)) continue;


                using (var s = selfAssembly.GetManifestResourceStream(resName))
                {
                    using (var t = new System.IO.StreamReader(s))
                    {
                        files.Add(t.ReadToEnd());
                    }
                }
            }

            return files.ToArray();
        }
    }


}