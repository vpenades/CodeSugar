using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;

#nullable disable

using __STREAMFUNC = System.Func<System.IO.FileMode, System.IO.Stream>;
using __STREAMTASK = System.Func<System.IO.FileMode, System.Threading.CancellationToken, System.Threading.Tasks.Task<System.IO.Stream>>;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    
    partial class CodeSugarExtensions    
    {
        private const System.IO.FileMode _MODEREAD = System.IO.FileMode.Open;
        private const System.IO.FileMode _MODEWRITE = System.IO.FileMode.Create;


        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static System.IO.Stream OpenRead(this __STREAMFUNC func)
        {
            var s = func?.Invoke(_MODEREAD);
            return s;
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static System.IO.Stream OpenWrite(this __STREAMFUNC func)
        {
            var s = func?.Invoke(_MODEWRITE);
            return s;
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static async Task<System.IO.Stream> OpenReadAsync(this __STREAMTASK func, CancellationToken token)
        {
            if (func == null) return null;
            var s = await func.Invoke(_MODEREAD, token).ConfigureAwait(false);
            return s;
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        public static async Task<System.IO.Stream> OpenWriteAsync(this __STREAMTASK func, CancellationToken token)
        {
            if (func == null) return null;
            var s = await func.Invoke(_MODEWRITE, token).ConfigureAwait(false);
            return s;
        }



        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        internal static __STREAMFUNC _ToStreamFunc(Func<System.IO.Stream> readStreamFunc, Func<System.IO.Stream> writeStreamFunc)
        {
            System.IO.Stream _Open(System.IO.FileMode mode)
            {
                switch (mode)
                {
                    case System.IO.FileMode.Open: return readStreamFunc?.Invoke() ?? throw new InvalidOperationException($"Unsupported: {mode}");
                    case System.IO.FileMode.Create: return writeStreamFunc?.Invoke() ?? throw new InvalidOperationException($"Unsupported: {mode}");
                    default: throw new InvalidOperationException($"Unsupported: {mode}");
                }
            }

            return _Open;
        }

        [System.Runtime.CompilerServices.MethodImpl(AGRESSIVE)]
        internal static __STREAMTASK _ToStreamTask(Func<CancellationToken, ValueTask<System.IO.Stream>> readStreamFunc, Func<CancellationToken, ValueTask<System.IO.Stream>> writeStreamFunc)
        {
            async Task<System.IO.Stream> _Open(System.IO.FileMode mode, CancellationToken token)
            {
                switch (mode)
                {
                    case System.IO.FileMode.Open:
                        {
                            var t = readStreamFunc?.Invoke(token) ?? throw new InvalidOperationException($"Unsupported: {mode}");
                            return await t.ConfigureAwait(false);
                        }
                    case System.IO.FileMode.Create:
                        {
                            var t = writeStreamFunc?.Invoke(token) ?? throw new InvalidOperationException($"Unsupported: {mode}");
                            return await t.ConfigureAwait(false);
                        }
                    default: throw new InvalidOperationException($"Unsupported: {mode}");
                }
            }

            return _Open;
        }
    }
}
