using System;
using System.Text;
using System.IO;
using System.Xml.Serialization;

#nullable disable

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;

using System.Diagnostics.CodeAnalysis;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        [return: NotNull]
        public static __STREAMFUNC GetStreamFunction([NotNull] this System.Reflection.Assembly assembly, string resourceName)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrWhiteSpace(nameof(resourceName))) throw new ArgumentNullException(nameof(resourceName));

            return _ToStreamFunc(() => assembly.GetManifestResourceStream(resourceName), null);
        }
    }
}
