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
        public static bool TryGetAlternateProgress<T,TAlt>(this IProgress<T> progress, out IProgress<TAlt> altProgress)
        {
            altProgress = null;

            if (progress is IProgress<TAlt> alt)
            {
                altProgress = alt;                
            }

            else if (__TryGetAlternateProgress(progress, typeof(IProgress<TAlt>), out var result))
            {
                altProgress = result as IProgress<TAlt>;
            }

            return altProgress != null;
        }

        /// <summary>
        /// Digs into <paramref name="progress"/> to find an alternate <paramref name="altProgress"/> interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="progress">The current progress interface.</param>
        /// <param name="altProgressType">The type of the alternate type to find.</param>
        /// <param name="altProgress">The alternate progress interface to find.</param>
        /// <returns>True if the alternate progress interface is found.</returns>
        public static bool TryGetAlternateProgress<T>(this IProgress<T> progress, Type altProgressType, out object altProgress)
        {
            return __TryGetAlternateProgress(progress, altProgressType, out altProgress);
        }

        private static bool __TryGetAlternateProgress(this object progress, Type altProgressType, out object altProgress)
        {
            altProgress = null;

            if (progress == null) return false;
            if (altProgressType == null) return false;

            if (altProgressType.IsAssignableFrom(progress.GetType()))
            {
                altProgress = progress;
            }
            else if (progress is IServiceProvider srv)
            {
                // recursively dig into more progress objects
                return __TryGetAlternateProgress(srv.GetService(altProgressType), altProgressType, out altProgress);
            }

            return altProgress != null;
        }

        /// <summary>
        /// Reports a percentaje and a message if the underlaying <see cref="IProgress{T}"/> supports it.
        /// </summary>
        /// <param name="percentProgress">The report target.</param>
        /// <param name="percent">A value between 0 and 100.</param>
        /// <param name="message">An optional message.</param>
        public static void ReportPercentAndMessage(this IProgress<int> percentProgress, int percent, string message)
        {
            if (percentProgress == null) return;

            System.Diagnostics.Debug.Assert(percent >= 0 && percent <= 100, $"Percent {percent} is out of bounds");

            if (message == null)
            {
                percentProgress.Report(percent);
                return;
            }

            // mixed progress
            if (percentProgress.TryGetAlternateProgress(out IProgress<(int,string)> mixedProgress))
            {
                mixedProgress.Report((percent, message));
                return;
            }

            // split progress
            percentProgress.Report(percent);
            if (percentProgress.TryGetAlternateProgress(out IProgress<string> textProgress))
            {
                textProgress.Report(message);
            }            
        }

        public static IProgress<int> GetPercentProgressPart(this IProgress<int> progress, int index, int count)
        {
            if (count <= 0) return new __PercentProgressConstant(progress, 50);

            if (index < 0) index = 0;
            if (index >= count) index = count - 1;

            return new __PercentProgressPart(progress, index, count);
        }

        private readonly struct __PercentProgressPart : IProgress<int>, IServiceProvider
        {
            public __PercentProgressPart(IProgress<int> parent, int index, int count)
            {
                _Parent = parent;
                _Index = index;
                _Count = count;
            }

            private readonly IProgress<int> _Parent;
            private readonly int _Index;
            private readonly int _Count;

            public void Report(int value)
            {
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                value += _Index * 100;
                value /= _Count;
                if (value < 0) value = 0;
                if (value > 100) value = 100;
                _Parent?.Report(value);
            }

            public object GetService(Type serviceType)
            {
                return _Parent.TryGetAlternateProgress(serviceType, out var altProgress)
                    ? altProgress
                    : null;
            }
        }

        private readonly struct __PercentProgressConstant : IProgress<int>, IServiceProvider
        {
            public __PercentProgressConstant(IProgress<int> parent, int value)
            {
                _Parent = parent;
                _Value = value;
            }

            private readonly IProgress<int> _Parent;
            private readonly int _Value;

            public void Report(int value)
            {
                _Parent?.Report(value);
            }

            public object GetService(Type serviceType)
            {
                return _Parent.TryGetAlternateProgress(serviceType, out var altProgress)
                    ? altProgress
                    : null;
            }
        }
        
    }
}
