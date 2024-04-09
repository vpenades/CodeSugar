// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    partial class CodeSugarForSystemIO
    {
        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA512"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha512(this FILE finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return s.ComputeSha512();
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA384"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha384(this FILE finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return s.ComputeSha384();
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.SHA256"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeSha256(this FILE finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return s.ComputeSha256();
            }
        }

        /// <summary>
        /// Computes the <see cref="System.Security.Cryptography.MD5"/> on the contents of the given file.
        /// </summary>
        public static Byte[] ComputeMd5(this FILE finfo)
        {
            GuardExists(finfo);

            using (var s = finfo.OpenRead())
            {
                return s.ComputeMd5();
            }
        }
    }
}