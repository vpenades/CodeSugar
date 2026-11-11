#if NET8_0_OR_GREATER

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

using Avalonia.Platform.Storage;

#nullable disable

// using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
// using __XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
// using __XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;

using __STREAMTASK = System.Func<System.IO.FileMode, System.Threading.CancellationToken, System.Threading.Tasks.Task<System.IO.Stream>>;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {

        public static bool TryGetDirectoryInfo(this Avalonia.Platform.Storage.IStorageFolder folder, out System.IO.DirectoryInfo dinfo)
        {
            dinfo = null;
            if (folder == null) return false;
            var path = folder.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(path)) return false;
            dinfo = new System.IO.DirectoryInfo(path);
            return true;
        }

        public static bool TryGetFileInfo(this Avalonia.Platform.Storage.IStorageFile file, out System.IO.FileInfo finfo)
        {
            finfo = null;
            if (file == null) return false;
            var path = file.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(path)) return false;
            finfo = new System.IO.FileInfo(path);
            return true;
        }

        public static __STREAMTASK GetStreamTask(this Avalonia.Platform.Storage.IStorageFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            async Task<System.IO.Stream> rsf(System.IO.FileMode mode, System.Threading.CancellationToken token)
            {
                switch(mode)
                {
                    case FileMode.Open: return await file.OpenReadAsync();
                    case FileMode.Create: return await file.OpenWriteAsync();
                    default: throw new NotSupportedException();
                }
            }

            return rsf;            
        }        

    }
}

#endif