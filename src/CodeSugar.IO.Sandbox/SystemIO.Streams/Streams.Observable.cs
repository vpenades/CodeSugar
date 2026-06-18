// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

#nullable disable

using __STREAM = System.IO.Stream;
using __BYTESSEGMENT = System.ArraySegment<byte>;

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static __STREAM WithDisposeObserver([AllowNull] this __STREAM stream, [AllowNull] Action onDispose)
        {
            if (onDispose == null) return stream;

            switch(stream)
            {
                case null: return null;

                // don't wrap streams that are already observable

                case _ObservableStream observable: 
                    observable.AddObserver(onDispose);
                    return observable;
                case _ObservableMemoryStream observable:
                    observable.AddObserver(bbb => onDispose.Invoke());
                    return observable;

                default: return new _ObservableStream(stream, onDispose);
            }
        }

        /// <summary>
        /// A <see cref="__STREAM"/> that, when closed, reports back to the host
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

            #region properties

            public override bool CanRead => _BaseStream().CanRead;
            public override bool CanSeek => _BaseStream().CanSeek;
            public override bool CanWrite => _BaseStream().CanWrite;
            public override long Length => _BaseStream().Length;

            public override long Position
            {
                get => _BaseStream().Position;
                set => _BaseStream().Position = value;
            }

            #endregion

            #region API

            public void AddObserver(Action onDispose)
            {
                if (_OnDispose == null) throw new ObjectDisposedException(nameof(_OnDispose));

                _OnDispose.Add(onDispose);
            }

            [return: NotNull]
            private __STREAM _BaseStream()
            {
                var stream = _Stream;

                return stream == null
                    ? throw new ObjectDisposedException(nameof(_Stream))
                    : stream;
            }            

            public override long Seek(long offset, SeekOrigin origin) { return _BaseStream().Seek(offset, origin); }
            public override void SetLength(long value) { _BaseStream().SetLength(value); }
            public override int Read(Span<byte> buffer) { return _BaseStream().Read(buffer); }
            public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            {
                return _BaseStream().ReadAsync(buffer, cancellationToken);
            }            
            public override int Read(byte[] buffer, int offset, int count) { return _BaseStream().Read(buffer, offset, count); }            
            public override void Write(ReadOnlySpan<byte> buffer) { _BaseStream().Write(buffer); }
            public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                return _BaseStream().WriteAsync(buffer, cancellationToken);
            }
            public override void Write(byte[] buffer, int offset, int count) { _BaseStream().Write(buffer, offset, count); }
            public override void Flush() { _BaseStream().Flush(); }            

            #endregion
        }

        /// <summary>
        /// A <see cref="MemoryStream"/> that, when closed, reports its underlaying data back to the host
        /// </summary>
        private sealed class _ObservableMemoryStream : System.IO.MemoryStream
        {
            #region lifecycle
            public _ObservableMemoryStream(Action<__BYTESSEGMENT> onClose)
            {
                _OnDispose.Add(onClose);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    var lambdas = System.Threading.Interlocked.Exchange(ref _OnDispose, null);

                    if (lambdas != null)
                    {
                        var buffer = this.TryGetBuffer(out var buff) ? buff : this.ToArray();

                        foreach(var lambda in lambdas) lambda.Invoke(buffer);
                    }
                }

                base.Dispose(disposing);
            }

            #endregion

            #region data

            private List<Action<__BYTESSEGMENT>> _OnDispose = new List<Action<__BYTESSEGMENT>>();

            #endregion

            #region API

            public void AddObserver(Action<__BYTESSEGMENT> onDispose)
            {
                if (_OnDispose == null) throw new ObjectDisposedException(nameof(_OnDispose));

                _OnDispose.Add(onDispose);
            }

            #endregion
        }
    }
}