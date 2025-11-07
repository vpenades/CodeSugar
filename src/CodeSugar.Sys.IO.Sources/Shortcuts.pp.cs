// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable disable

using __FINFO = System.IO.FileInfo;
using __DINFO = System.IO.DirectoryInfo;
using __SINFO = System.IO.FileSystemInfo;


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
        public static void WriteShortcut(this __FINFO finfo, __SINFO target)
        {
            GuardNotNull(finfo);
            GuardNotNull(target);

            var uri = new Uri(target.FullName, UriKind.Absolute);
            WriteShortcut(finfo, uri);
        }

        public static void WriteShortcut(this __FINFO finfo, Uri uri)
        {
            GuardNotNull(finfo);

            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri) throw new ArgumentException("relative uris are not supported", nameof(uri)); // windows simply redirects to Bing.

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

        
        public static Uri ResolveShortcutUri(this __FINFO shortcutFile)
        {
            GuardExists(shortcutFile);            

            HashSet<__FINFO> circularBarrier = null; 

            while(true)            
            {
                if (circularBarrier?.Contains(shortcutFile) ?? false) throw new ArgumentException("circular shortcut detected.",nameof(shortcutFile));

                var uri = ReadShortcutUri(shortcutFile);                

                if (!uri.IsFile) return uri;
                if (!uri.LocalPath.EndsWith(".url",StringComparison.OrdinalIgnoreCase)) return uri;                
                
                // keep digging:

                circularBarrier ??= new HashSet<__FINFO>(MatchCasing.PlatformDefault.GetFullNameComparer<__FINFO>());
                circularBarrier.Add(shortcutFile);

                shortcutFile = new __FINFO(uri.LocalPath);                
            }
        }

        public static __FINFO ResolveShortcutFile(this __FINFO file)
        {
            return TryResolveShortcutFile(file, out var newFile) ? newFile : file;
        }

        public static bool TryResolveShortcutFile(this __FINFO shortcutOrFile, out __FINFO actualFile)
        {
            GuardNotNull(shortcutOrFile);

            actualFile = null;

            if (!shortcutOrFile.HasExtension(".url")) // it's not a shortcut, or the shortcut has a close enough name
            {
                if (shortcutOrFile.RefreshedExists()) { actualFile = shortcutOrFile; return true; } // no shortcut at all.

                // try alternate shortcut name
                var altFile = shortcutOrFile.FullName + ".url";
                if (TryResolveShortcutFile(new __FINFO(altFile), out actualFile)) return true;

                // try alternate shortcut name
                altFile = System.IO.Path.ChangeExtension(shortcutOrFile.FullName , ".url");
                if (TryResolveShortcutFile(new __FINFO(altFile), out actualFile)) return true;

                return false;
            }            

            HashSet<__FINFO> circularBarrier = null;

            while(true)
            {
                if (!shortcutOrFile.RefreshedExists()) return false;

                if (circularBarrier?.Contains(shortcutOrFile) ?? false) throw new ArgumentException("circular shortcut detected.",nameof(shortcutOrFile));                

                var uri = ReadShortcutUri(shortcutOrFile);                

                if (!uri.IsFile) return false; // not a file

                var file = new __FINFO(uri.LocalPath);

                if (!file.HasExtension(".url")) { actualFile = file; return true; }
                
                // keep digging:

                circularBarrier ??= new HashSet<__FINFO>(MatchCasing.PlatformDefault.GetFullNameComparer<__FINFO>());
                circularBarrier.Add(shortcutOrFile);

                shortcutOrFile = file;                
            }
        }

        public static bool TryResolveShortcutDir(this __SINFO shortcutOrDir, out __DINFO actualDirectory)
        {
            GuardNotNull(shortcutOrDir);

            actualDirectory = null;

            HashSet<__SINFO> circularBarrier = null;            

            while(true)
            {
                if (!shortcutOrDir.RefreshedExists()) return false;

                if (shortcutOrDir is __DINFO dir) { actualDirectory = dir; return true; }

                if (circularBarrier?.Contains(shortcutOrDir) ?? false) throw new ArgumentException("circular shortcut detected.",nameof(shortcutOrDir));

                if (!(shortcutOrDir is __FINFO file)) return false;

                var uri = ReadShortcutUri(file);

                if (!uri.IsFile) return false; // not a file or dir

                if (!uri.LocalPath.EndsWith(".url",StringComparison.OrdinalIgnoreCase))
                {
                    actualDirectory = new __DINFO(uri.LocalPath);
                    return true;
                }
                
                // keep digging:

                circularBarrier ??= new HashSet<__SINFO>(MatchCasing.PlatformDefault.GetFullNameComparer<__SINFO>());
                circularBarrier.Add(shortcutOrDir);

                shortcutOrDir = new __FINFO(uri.LocalPath);                
            }
        }

        public static bool TryReadShortcutFile(this __FINFO shortcutFile, out __FINFO targetFile)
        {
            var uri = ReadShortcutUri(shortcutFile);

            targetFile = null;
            if (uri == null) return false;
            if (!uri.IsFile) return false;

            targetFile = new __FINFO(uri.LocalPath);
            return targetFile.CachedExists();
        }

        public static bool TryReadShortcutDir(this __FINFO shortcutFile, out __DINFO targetDirectory)
        {
            var uri = ReadShortcutUri(shortcutFile);

            targetDirectory = null;
            if (uri == null) return false;
            if (!uri.IsFile) return false;

            targetDirectory = new __DINFO(uri.LocalPath);
            return targetDirectory.CachedExists();
        }

        public static Uri ReadShortcutUri(this __FINFO finfo)
        {
            GuardNotNull(finfo);

            _CheckShortcutNotTemp(finfo);

            var lines = ReadAllLines(finfo);

            var line = lines.FirstOrDefault(l=> l.StartsWith("URL="));
            if (line == null) return null;
            line = line.Trim();
            if (line.Length < 5) return null;

            line = line.Substring(4); // remove "URL="

            line = line.Trim();

            if (!Uri.TryCreate(line, UriKind.Absolute, out Uri uri)) return null;

            if (uri.IsFile)
            {
                var uriPath = uri.LocalPath;
                
            }

            return uri;
        }

        private static void _CheckShortcutNotTemp(__SINFO target)
        {
            if (target is __FINFO file) target = file.Directory;
            if (target is __DINFO dir && dir.IsTemp()) throw new System.Security.SecurityException("Shortcut resolution disallowed for Temp directory");
        }
    }
}