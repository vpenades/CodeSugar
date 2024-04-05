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
using FILEORDIR = System.IO.FileSystemInfo;


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
        public static void WriteShortcut(this FILE finfo, FILEORDIR target)
        {
            GuardNotNull(finfo);
            GuardNotNull(target);

            var uri = new Uri(target.FullName, UriKind.Absolute);
            WriteShortcut(finfo, uri);
        }

        public static void WriteShortcut(this FILE finfo, Uri uri)
        {
            GuardNotNull(finfo);

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

            HashSet<FILE> circularBarrier = null; 

            while(true)            
            {
                if (circularBarrier?.Contains(shortcutFile) ?? false) throw new ArgumentException("circular shortcut detected.",nameof(shortcutFile));

                var uri = ReadShortcutUri(shortcutFile);                

                if (!uri.IsFile) return uri;
                if (!uri.LocalPath.EndsWith(".url",StringComparison.OrdinalIgnoreCase)) return uri;                
                
                // keep digging:

                circularBarrier ??= new HashSet<FILE>(MatchCasing.PlatformDefault.GetFullNameComparer<FILE>());
                circularBarrier.Add(shortcutFile);

                shortcutFile = new FILE(uri.LocalPath);                
            }
        }

        public static FILE ResolveShortcutFile(this FILE file)
        {
            return TryResolveShortcutFile(file, out var newFile) ? newFile : file;
        }

        public static bool TryResolveShortcutFile(this FILE shortcutOrFile, out FILE actualFile)
        {
            GuardNotNull(shortcutOrFile);

            actualFile = null;

            if (!shortcutOrFile.HasExtension(".url")) // it's not a shortcut, or the shortcut has a close enough name
            {
                if (shortcutOrFile.Exists) { actualFile = shortcutOrFile; return true; } // no shortcut at all.

                // try alternate shortcut name
                var altFile = shortcutOrFile.FullName + ".url";
                if (TryResolveShortcutFile(new FILE(altFile), out actualFile)) return true;

                // try alternate shortcut name
                altFile = System.IO.Path.ChangeExtension(shortcutOrFile.FullName , ".url");
                if (TryResolveShortcutFile(new FILE(altFile), out actualFile)) return true;

                return false;
            }            

            HashSet<FILE> circularBarrier = null;

            while(true)
            {
                if (!shortcutOrFile.Exists) return false;

                if (circularBarrier?.Contains(shortcutOrFile) ?? false) throw new ArgumentException("circular shortcut detected.",nameof(shortcutOrFile));                

                var uri = ReadShortcutUri(shortcutOrFile);                

                if (!uri.IsFile) return false; // not a file

                var file = new FILE(uri.LocalPath);

                if (!file.HasExtension(".url")) { actualFile = file; return true; }
                
                // keep digging:

                circularBarrier ??= new HashSet<FILE>(MatchCasing.PlatformDefault.GetFullNameComparer<FILE>());
                circularBarrier.Add(shortcutOrFile);

                shortcutOrFile = file;                
            }
        }

        public static bool TryResolveShortcutDir(this FILEORDIR shortcutOrDir, out DIRECTORY actualDirectory)
        {
            GuardNotNull(shortcutOrDir);

            actualDirectory = null;

            HashSet<FILEORDIR> circularBarrier = null;            

            while(true)
            {
                if (!shortcutOrDir.Exists()) return false;

                if (shortcutOrDir is DIRECTORY dir) { actualDirectory = dir; return true; }

                if (circularBarrier?.Contains(shortcutOrDir) ?? false) throw new ArgumentException("circular shortcut detected.",nameof(shortcutOrDir));

                if (!(shortcutOrDir is FILE file)) return false;

                var uri = ReadShortcutUri(file);

                if (!uri.IsFile) return false; // not a file or dir

                if (!uri.LocalPath.EndsWith(".url",StringComparison.OrdinalIgnoreCase))
                {
                    actualDirectory = new DIRECTORY(uri.LocalPath);
                    return true;
                }
                
                // keep digging:

                circularBarrier ??= new HashSet<FILEORDIR>(MatchCasing.PlatformDefault.GetFullNameComparer<FILEORDIR>());
                circularBarrier.Add(shortcutOrDir);

                shortcutOrDir = new FILE(uri.LocalPath);                
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