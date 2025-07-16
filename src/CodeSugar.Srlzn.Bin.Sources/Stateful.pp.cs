// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable disable

using _STREAM = System.IO.Stream;

using System.Diagnostics.CodeAnalysis;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForSerialization
    {
        [return: NotNull]
        public static IProgress<Byte> AsStatefulProgressWriter<TList>([NotNull] this TList list, int position = 0)
            where TList : IList<byte>
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            return new _StatefulListWriter<TList>(list, position);
        }

        private sealed class _StatefulListWriter<TList> : IProgress<byte>
            where TList : IList<byte>
        {
            public _StatefulListWriter(TList list, int position)
            {
                _List = list;
                _Position = position;
            }

            private readonly TList _List;
            private int _Position;

            public void Report(byte value)
            {
                while(_Position >= _List.Count)
                {
                    _List.Add(0);
                }

                _List[_Position++] = value;
            }
        }        
    }
}
