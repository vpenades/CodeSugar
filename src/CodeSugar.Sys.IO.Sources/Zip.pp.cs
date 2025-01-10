// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using ZIPARCHIVE = System.IO.Compression.ZipArchive;
using ZIPENTRY = System.IO.Compression.ZipArchiveEntry;
using BYTESSEGMENT = System.ArraySegment<byte>;

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
        #if !NET

        public static void GuardReadable(this ZIPENTRY entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            if (entry.Archive.Mode != System.IO.Compression.ZipArchiveMode.Read) throw new ArgumentException("Can't read from strean", nameof(entry));
        }

        public static void GuardWriteable(this ZIPENTRY entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));
            if (entry.Archive.Mode != System.IO.Compression.ZipArchiveMode.Create) throw new ArgumentException("Can't read from strean", nameof(entry));
        }

        #else

        public static void GuardReadable(this ZIPENTRY entry, [CallerArgumentExpression("entry")] string name = null)
        {
            if (entry == null) throw new ArgumentNullException(name);
            if (entry.Archive.Mode != System.IO.Compression.ZipArchiveMode.Read) throw new ArgumentException("Can't read from strean", name);
        }

        public static void GuardWriteable(this ZIPENTRY entry, [CallerArgumentExpression("entry")] string name = null)
        {
            if (entry == null) throw new ArgumentNullException(name);
            if (entry.Archive.Mode != System.IO.Compression.ZipArchiveMode.Create) throw new ArgumentException("Can't read from strean", name);
        }

        #endif

        public static ZIPARCHIVE CreateZipArchive(this System.IO.FileInfo finfo, System.Text.Encoding entryNameEncoding = null)
        {
            GuardNotNull(finfo);
            if (finfo.Exists) finfo.Delete(); // zip create fails if it already exists
            return System.IO.Compression.ZipFile.Open(finfo.FullName, System.IO.Compression.ZipArchiveMode.Create, entryNameEncoding);
        }

        public static ZIPARCHIVE OpenReadZipArchive(this System.IO.FileInfo finfo, System.Text.Encoding entryNameEncoding = null)
        {
            GuardExists(finfo);
            return System.IO.Compression.ZipFile.Open(finfo.FullName, System.IO.Compression.ZipArchiveMode.Read, entryNameEncoding);
        }

        public static Dictionary<string,BYTESSEGMENT> ToDictionary(this ZIPARCHIVE archive)
        {
            if (archive == null) throw new ArgumentNullException(nameof(archive));

            return archive.Entries.ToDictionary(entry => entry.FullName, entry => entry.ReadAllBytes());
        }

        public static void AddEntries(this ZIPARCHIVE archive, IReadOnlyDictionary<string,BYTESSEGMENT> entries)
        {
            if (archive == null) throw new ArgumentNullException(nameof(archive));
            if (entries == null) throw new ArgumentNullException(nameof(entries));
            
            foreach(var entry in entries)
            {
                var zipPath = entry.Key.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
                archive.CreateEntry(zipPath).WriteAllBytes(entry.Value);
            }        
        }
    }
}
