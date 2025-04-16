// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    static partial class CodeSugarForFileProviders
    {
        #region constants

        private static readonly _XINFO __NULLFILE = new Microsoft.Extensions.FileProviders.NotFoundFileInfo("NULL");

        private static readonly _XPROVIDER __NULLPROVIDER = new Microsoft.Extensions.FileProviders.NullFileProvider();

        #endregion

        public static Func<System.IO.Stream> GetReadStreamFunction(this _XINFO xinfo)
        {
            GuardNotNull(xinfo);
            if (xinfo.IsDirectory) throw new ArgumentException("directories don't have a stream", nameof(xinfo));

            return xinfo.CreateReadStream;
        }

        public static Func<System.IO.Stream> GetWriteStreamFunction(this _XINFO xinfo)
        {
            GuardNotNull(xinfo);
            if (xinfo.IsDirectory) throw new ArgumentException("directories don't have a stream", nameof(xinfo));

            if (!string.IsNullOrWhiteSpace(xinfo.PhysicalPath))
            {
                return new System.IO.FileInfo(xinfo.PhysicalPath).OpenWrite;
            }            

            // TODO: handle xinfo's IServiceProvider

            return null;
        }
    }
}
