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
        public static void WriteShortcut(this FILE finfo, System.IO.FileSystemInfo target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var uri = new Uri(target.FullName, UriKind.Absolute);
            WriteShortcut(finfo, uri);
        }

        public static void WriteShortcut(this FILE finfo, Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            if (uri.IsFile)
            {
                WriteAllLines(finfo, null,
                    "[{000214A0-0000-0000-C000-000000000046}]",
                    "Prop3=19,11",
                    "[InternetShortcut]",
                    "IDList=",
                    $"URL={uri.AbsoluteUri}",
                    "IconIndex=1",
                    "IconFile=" + uri.LocalPath.Replace('\\', '/')
                    );
            }
            else
            {
                WriteAllLines(finfo, null,
                    "[{000214A0-0000-0000-C000-000000000046}]",
                    "Prop3=19,11",
                    "[InternetShortcut]",
                    "IDList=",
                    $"URL={uri.AbsoluteUri}",
                    "IconIndex=0"
                    );
            }
        }

        public static Uri ResolveShortcutUri(this FILE shortcutFile)        
        {
            GuardExists(shortcutFile);

            var circularBarrier = new HashSet<FILE>(GetFullNameComparer<FILE>());

            while(true)            
            {
                if (circularBarrier.Contains(shortcutFile)) throw new ArgumentException("circular shortcut detected.",nameof(shortcutFile));

                var uri = ReadShortcutUri(shortcutFile);

                circularBarrier.Add(shortcutFile);

                if (!uri.IsFile) return uri;
                if (!uri.LocalPath.EndsWith(".url",StringComparison.OrdinalIgnoreCase)) return uri;
                
                // keep digging:
                shortcutFile = new FILE(uri.LocalPath);

                // TODO: check circular
            }
        }

        public static FILE ResolveShortcutFile(this FILE shortcutFile)
        {
            GuardExists(shortcutFile);

            if (!shortcutFile.Name.EndsWith(".url",StringComparison.OrdinalIgnoreCase)) return shortcutFile;

            var circularBarrier = new HashSet<FILE>(GetFullNameComparer<FILE>());

            while(true)
            {
                if (circularBarrier.Contains(shortcutFile)) throw new ArgumentException("circular shortcut detected.",nameof(shortcutFile));

                var uri = ReadShortcutUri(shortcutFile);

                circularBarrier.Add(shortcutFile);

                if (!uri.IsFile) throw new ArgumentException($"{uri} not a file url", nameof(shortcutFile));
                if (!uri.LocalPath.EndsWith(".url",StringComparison.OrdinalIgnoreCase)) return new FILE(uri.LocalPath);
                
                // keep digging:
                shortcutFile = new FILE(uri.LocalPath);

                // TODO: check circular
            }
        }

        public static DIRECTORY ResolveShortcutDir(this FILE shortcutFile)
        {
            GuardExists(shortcutFile);
            
            var circularBarrier = new HashSet<FILE>(GetFullNameComparer<FILE>());

            while(true)
            {
                if (circularBarrier.Contains(shortcutFile)) throw new ArgumentException("circular shortcut detected.",nameof(shortcutFile));

                var uri = ReadShortcutUri(shortcutFile);

                circularBarrier.Add(shortcutFile);

                if (!uri.IsFile) throw new ArgumentException($"{uri} not a file url", nameof(shortcutFile));
                if (!uri.LocalPath.EndsWith(".url",StringComparison.OrdinalIgnoreCase)) return new DIRECTORY(uri.LocalPath);
                
                // keep digging:
                shortcutFile = new FILE(uri.LocalPath);

                // TODO: check circular
            }
        }

        public static bool TryReadShortcutFile(this FILE shortcutFile, out FILE targetFile)
        {
            var uri = ReadShortcutUri(shortcutFile);

            targetFile = null;
            if (uri == null) return false;
            if (!uri.IsFile) return false;

            targetFile = new FILE(uri.LocalPath);
            return targetFile.Exists;
        }

        public static bool TryReadShortcutDir(this FILE shortcutFile, out DIRECTORY targetDirectory)
        {
            var uri = ReadShortcutUri(shortcutFile);

            targetDirectory = null;
            if (uri == null) return false;
            if (!uri.IsFile) return false;

            targetDirectory = new DIRECTORY(uri.LocalPath);
            return targetDirectory.Exists;
        }

        public static Uri ReadShortcutUri(this FILE finfo)
        {
            var lines = ReadAllLines(finfo);

            var line = lines.FirstOrDefault(l=> l.StartsWith("URL="));
            if (line == null) return null;
            line = line.Trim();
            if (line.Length < 5) return null;

            line = line.Substring(4); // remove "URL="

            line = line.Trim();

            return Uri.TryCreate(line, UriKind.Absolute, out Uri uri) ? uri : null;        
        }        
    }
}