// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Linq;
using System.IO.Compression;

#nullable disable


#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    internal static partial class CodeSugarForFileProviders
    {
        [return: NotNull]
        private static System.IO.MemoryStream _ToMemoryStream(ArraySegment<byte> segment)
        {
            return new MemoryStream(segment.Array ?? Array.Empty<byte>(), segment.Offset, segment.Count, false);
        }

        private class _ObservableMemoryStream : System.IO.MemoryStream
        {
            public _ObservableMemoryStream(Action<ArraySegment<byte>> onClose)
            {
                _OnClose = onClose;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                var lambda = System.Threading.Interlocked.Exchange(ref _OnClose, null);

                if (lambda != null)
                {
                    var buffer = this.TryGetBuffer(out var buff) ? buff : this.ToArray();

                    lambda.Invoke(buffer);
                }                
            }

            private Action<ArraySegment<byte>> _OnClose;            
        }
    }
}
