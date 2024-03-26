// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using FILE = System.IO.FileInfo;
using DIRECTORY = System.IO.DirectoryInfo;
using SYSTEMENTRY = System.IO.FileSystemInfo;

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
        /// Gets a formatter that converts a byte size number into a human friendly size.
        /// </summary>
        public static IFormatProvider FileSizeFormatter => _FileSizeFormatProvider.Default;

        
        private sealed class _FileSizeFormatProvider : IFormatProvider, ICustomFormatter
        {
            public static IFormatProvider Default { get; } = new _FileSizeFormatProvider();

            // Define the file size units
            private static readonly string[] fileSizeUnits = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            // Get the format provider for the specified type
            public object GetFormat(Type formatType)
            {
                return formatType == typeof(ICustomFormatter) ? this : (object)null;
            }
        
            // Format the file size in a user-friendly way
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg is IConvertible convertible)
                {
                    arg = convertible.ToDouble(formatProvider);
                }

                // Check if the argument is a double value
                if (arg is double valDouble)
                {
                    bool isNegative = valDouble < 0;
                    if (isNegative) valDouble = -valDouble;

                    // Find the appropriate unit index
                    int unitIndex = 0;
                    while (valDouble >= 1024 && unitIndex < fileSizeUnits.Length)
                    {
                        valDouble /= 1024;
                        unitIndex++;
                    }

                    // Return the formatted file size with the unit

                    var text = (isNegative ? "-" : string.Empty);
                    text += string.Format("{0:0.##} {1}", valDouble, fileSizeUnits[unitIndex]);
                    return text;
                }
                // If the argument is not a long value, use the default format provider
                else
                {
                    if (arg is IFormattable formattable) return formattable.ToString(format, System.Globalization.CultureInfo.CurrentCulture);
                    else if (arg != null) return arg.ToString();
                    else return string.Empty;
                }
            }
        }
    }
}
