using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#nullable disable

namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static void SetProgress([AllowNull] this SharpCompress.Readers.ReaderOptions readerOptions, [AllowNull] IProgress<int> progress)
        {
            if (readerOptions == null) return;
            if (progress == null) return;

            void _OnReport(SharpCompress.Common.ProgressReport report)
            {
                progress.Report((int)(report.PercentComplete ?? 0));
            }

            readerOptions.Progress = new Progress<SharpCompress.Common.ProgressReport>(_OnReport);
        }
    }
}
