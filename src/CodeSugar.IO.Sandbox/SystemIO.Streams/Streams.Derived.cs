// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#nullable disable

using __STREAM = System.IO.Stream;
using __BYTESSEGMENT = System.ArraySegment<byte>;
using System.Diagnostics.CodeAnalysis;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static __STREAM WithDisposeObserver(this __STREAM stream, Action onDispose)
        {
            if (onDispose == null) return stream;

            switch(stream)
            {
                case null:return null;                
                case _ObservableStream observable: // don't wrap multiple observers
                    observable.AddObserver(onDispose);
                    return observable;

                default: return new _ObservableStream(stream, onDispose);
            }
        }

        /// <summary>
        /// A <see cref="Stream"/> that, when closed, reports back to the host
        /// </summary>
        private sealed class _ObservableStream : __STREAM
        {
            #region lifecycle
            public _ObservableStream(__STREAM strean, Action onDispose)
            {
                _Stream = strean;
                _OnDispose.Add(onDispose);                
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    try
                    {
                        var lambdas = System.Threading.Interlocked.Exchange(ref _OnDispose, null);
                        if (lambdas != null)
                        {
                            foreach (var lambda in lambdas) lambda.Invoke();
                        }
                    }
                    finally
                    {
                        var stream = System.Threading.Interlocked.Exchange(ref _Stream, null);
                        stream?.Dispose();
                    }
                }

                base.Dispose(disposing);                
            }

            #endregion

            #region data

            private List<Action> _OnDispose = new List<Action>();
            private __STREAM _Stream;

            #endregion

            #region API

            public void AddObserver(Action onDispose)
            {
                if (_OnDispose == null) throw new ObjectDisposedException(nameof(_OnDispose));

                _OnDispose.Add(onDispose);
            }

            [return: NotNull]
            private __STREAM _UsingStream()
            {
                return _Stream == null ? throw new ObjectDisposedException(nameof(_Stream)) : _Stream;
            }

            public override bool CanRead => _UsingStream().CanRead;
            public override bool CanSeek => _UsingStream().CanSeek;
            public override bool CanWrite => _UsingStream().CanWrite;
            public override long Length => _UsingStream().Length;

            public override long Position
            {
                get => _UsingStream().Position;
                set => _UsingStream().Position = value;
            }

            public override long Seek(long offset, SeekOrigin origin) { return _UsingStream().Seek(offset, origin); }
            public override void SetLength(long value) { _UsingStream().SetLength(value); }
            public override int Read(Span<byte> buffer) { return _UsingStream().Read(buffer); }
            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                return _UsingStream().ReadAsync(buffer, cancellationToken);
            }            
            public override int Read(byte[] buffer, int offset, int count) { return _UsingStream().Read(buffer, offset, count); }            
            public override void Write(ReadOnlySpan<byte> buffer) { _UsingStream().Write(buffer); }
            public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                return _UsingStream().WriteAsync(buffer, cancellationToken);
            }
            public override void Write(byte[] buffer, int offset, int count) { _UsingStream().Write(buffer, offset, count); }
            public override void Flush() { _UsingStream().Flush(); }            

            #endregion
        }

        /// <summary>
        /// A <see cref="MemoryStream"/> that, when closed, reports its underlaying data to the host
        /// </summary>
        private sealed class _ObservableMemoryStream : System.IO.MemoryStream
        {
            #region lifecycle
            public _ObservableMemoryStream(Action<__BYTESSEGMENT> onClose)
            {
                _OnClose = onClose;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    var lambda = System.Threading.Interlocked.Exchange(ref _OnClose, null);

                    if (lambda != null)
                    {
                        var buffer = this.TryGetBuffer(out var buff) ? buff : this.ToArray();

                        lambda.Invoke(buffer);
                    }
                }

                base.Dispose(disposing);
            }

            #endregion

            #region data

            private Action<__BYTESSEGMENT> _OnClose;

            #endregion
        }
    }
}