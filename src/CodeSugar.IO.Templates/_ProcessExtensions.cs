using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        #if NETSTANDARD

        private static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            if (process == null) throw new ArgumentNullException(nameof(process));

            // If the process has already exited, complete immediately
            if (process.HasExited)
            {
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            EventHandler exitHandler = null;
            exitHandler = (sender, e) =>
            {
                process.Exited -= exitHandler;
                tcs.TrySetResult(null);
            };

            process.EnableRaisingEvents = true;
            process.Exited += exitHandler;

            // Check again after wire-up to avoid a race condition where the process exited 
            // right before the event handler was attached.
            if (process.HasExited)
            {
                process.Exited -= exitHandler;
                return Task.CompletedTask;
            }

            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    process.Exited -= exitHandler;
                    tcs.TrySetCanceled(cancellationToken);
                });
            }

            return tcs.Task;
        }

        #endif
    }
}
