// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystem
    {
        /// <summary>
        /// Digs into <paramref name="progress"/> to find an alternate <paramref name="altProgress"/> interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAlt"></typeparam>
        /// <param name="progress">The current progress interface.</param>
        /// <param name="altProgress">The alternate progress interface to find.</param>
        /// <returns>True if the alternate progress interface is found.</returns>
        public static bool TryGetAltProgress<T,TAlt>(this IProgress<T> progress, out IProgress<TAlt> altProgress)
        {
            if (progress is IProgress<TAlt> alt)
            {
                altProgress = alt;
                return true;
            }

            if (progress is IServiceProvider srv)
            {
                altProgress = srv.GetService(typeof(IServiceProvider)) as IProgress<TAlt>;
                if (altProgress != null) return true;
            }

            altProgress = null;
            return false;
        }

        /// <summary>
        /// Reports a percentaje and a message if the underlaying <see cref="IProgress{T}"/> supports it.
        /// </summary>
        /// <param name="percentProgress">The report target.</param>
        /// <param name="percent">A value between 0 and 100.</param>
        /// <param name="message">An optional message.</param>
        public static void ReportEx(this IProgress<int> percentProgress, int percent, string message)
        {
            if (percentProgress == null) return;

            System.Diagnostics.Debug.Assert(percent >= 0 && percent <= 100, $"Percent {percent} is out of bounds");

            if (message == null)
            {
                percentProgress.Report(percent);
                return;
            }

            if (percentProgress.TryGetAltProgress(out IProgress<(int,string)> mixedProgress))
            {
                mixedProgress.Report((percent, message));
                return;
            }            

            percentProgress.Report(percent);
            if (percentProgress is IProgress<string> textProgress) textProgress.Report(message);
        }
    }
}
