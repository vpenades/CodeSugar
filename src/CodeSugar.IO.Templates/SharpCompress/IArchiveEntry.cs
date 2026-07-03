using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable

using __SCARCHENTRY = SharpCompress.Archives.IArchiveEntry;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static void GuardReadable(this __SCARCHENTRY entry, [CallerArgumentExpression(nameof(entry))] string name = null)
        {
            if (entry == null) throw new ArgumentNullException(name);
            if (entry.IsDirectory) throw new ArgumentException("not a file", name);
            // todo: check that the archive is readable
        }

        public static void GuardWriteable(this __SCARCHENTRY entry, [CallerArgumentExpression(nameof(entry))] string name = null)
        {
            if (entry == null) throw new ArgumentNullException(name);
            if (entry.IsDirectory) throw new ArgumentException("not a file", name);
            // todo: check that the archive is writeable
        }

        [return: NotNull]
        public static Func<System.IO.Stream> GetReadStreamFunction([NotNull] this __SCARCHENTRY entry)
        {
            GuardReadable(entry);
            return entry.OpenEntryStream;
        }

        [return: NotNull]
        public static Func<System.IO.Stream> GetWriteStreamFunction([NotNull] this __SCARCHENTRY entry)
        {
            GuardWriteable(entry);            
            return entry.OpenEntryStream;
        }        
    }
}
