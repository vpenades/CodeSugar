using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;


#nullable disable

using __SCARCHENTRY = SharpCompress.Archives.IArchiveEntry;

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;
using __STREAMTASK = System.Func<System.IO.FileMode, System.Threading.CancellationToken, System.Threading.Tasks.Task<System.IO.Stream>>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static __SCARCHENTRY GuardReadable(this __SCARCHENTRY entry, [CallerArgumentExpression(nameof(entry))] string name = null)
        {
            if (entry == null) throw new ArgumentNullException(name);
            if (entry.IsDirectory) throw new ArgumentException("not a file", name);
            // todo: check that the archive is readable

            return entry;
        }

        public static __SCARCHENTRY GuardWriteable(this __SCARCHENTRY entry, [CallerArgumentExpression(nameof(entry))] string name = null)
        {
            if (entry == null) throw new ArgumentNullException(name);
            if (entry.IsDirectory) throw new ArgumentException("not a file", name);
            // todo: check that the archive is writeable

            return entry;
        }

        [return: NotNull]
        public static __STREAMFUNC GetStreamFunction([NotNull] this __SCARCHENTRY entry)
        {
            if (entry.IsDirectory) throw new ArgumentException("not a file", nameof(entry));            

            return _ToStreamFunc(entry.OpenEntryStream, entry.OpenEntryStream);
        }

        [return: NotNull]
        public static __STREAMTASK GetStreamTask([NotNull] this __SCARCHENTRY entry)
        {
            if (entry.IsDirectory) throw new ArgumentException("not a file", nameof(entry));

            return _ToStreamTask(entry.OpenEntryStreamAsync, entry.OpenEntryStreamAsync);
        }
    }
}
