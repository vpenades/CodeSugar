// GENERATOR_REQUIRES: Avalonia

#if NET8_0_OR_GREATER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Platform.Storage;

#if __REFERENCES_MICROSOFTEXTENSIONSFILEPROVIDERSABSTRACTIONS
using Microsoft.Extensions.FileProviders;

using __XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using __XCONTENTS = Microsoft.Extensions.FileProviders.IDirectoryContents;
#endif

#nullable disable

using __AVALONIASTORAGEITEM = Avalonia.Platform.Storage.IStorageItem;
using __AVALONIASTORAGEFILE = Avalonia.Platform.Storage.IStorageFile;
using __AVALONIASTORAGEFOLDER = Avalonia.Platform.Storage.IStorageFolder;

using __STREAMTASK = System.Func<System.IO.FileMode, System.Threading.CancellationToken, System.Threading.Tasks.Task<System.IO.Stream>>;


namespace __CODESUGAR_ROOTNAMESPACE__
{
    partial class CodeSugarExtensions
    {
        public static bool TryGetFileSystemInfo(this __AVALONIASTORAGEITEM storage, [NotNullWhen(true)] out System.IO.FileSystemInfo sinfo)
        {
            switch (storage)
            {
                case null: sinfo = null; return false;
                case __AVALONIASTORAGEFILE file when TryGetFileInfo(file, out var finfo): sinfo = finfo; return true;
                case __AVALONIASTORAGEFOLDER folder when TryGetDirectoryInfo(folder, out var dinfo): sinfo = dinfo; return true;
                default: throw new NotImplementedException(storage.GetType().Name);
            }
        }

        public static bool TryGetDirectoryInfo(this __AVALONIASTORAGEFOLDER folder, out System.IO.DirectoryInfo dinfo)
        {
            dinfo = null;
            if (folder == null) return false;
            var path = folder.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(path)) return false;
            dinfo = new System.IO.DirectoryInfo(path);
            return true;
        }

        public static bool TryGetFileInfo(this __AVALONIASTORAGEFILE file, out System.IO.FileInfo finfo)
        {
            finfo = null;
            if (file == null) return false;
            var path = file.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(path)) return false;
            finfo = new System.IO.FileInfo(path);
            return true;
        }

        public static __STREAMTASK GetStreamTask(this __AVALONIASTORAGEFILE file)
        {
            ArgumentNullException.ThrowIfNull(file);

            async Task<System.IO.Stream> rsf(System.IO.FileMode mode, System.Threading.CancellationToken token)
            {
                switch (mode)
                {
                    case FileMode.Open: return await file.OpenReadAsync();
                    case FileMode.Create: return await file.OpenWriteAsync();
                    default: throw new NotSupportedException();
                }
            }

            return rsf;
        }

        #if __REFERENCES_MICROSOFTEXTENSIONSFILEPROVIDERSABSTRACTIONS

        public static __XINFO ToIFileInfo(this __AVALONIASTORAGEITEM storage)
        {
            switch(storage)
            {
                case null: return new Microsoft.Extensions.FileProviders.NotFoundFileInfo("?");

                case __AVALONIASTORAGEFILE file:
                    return TryGetFileInfo(file, out var finfo)
                        ? finfo.ToIFileInfo()
                        : new _IStorageFileAsIFileInfo(file);

                case __AVALONIASTORAGEFOLDER folder:
                    return TryGetDirectoryInfo(folder, out var dinfo)
                        ? dinfo.ToIFileInfo()
                        : new _IStorageFolderAsIFileInfo(folder);

                default: throw new NotImplementedException(storage.GetType().Name);
            }            
        }


        [System.Diagnostics.DebuggerDisplay("_FileInfo {Name}")]
        internal class _IStorageFileAsIFileInfo : __XINFO
        {
            #region lifecycle
            public _IStorageFileAsIFileInfo(__AVALONIASTORAGEFILE file)
            {
                _File = file;
                LastModified = DateTime.Now; // it may be possible to retrieve datetime and legth from _File.GetBasicPropertiesAsync
            }

            #endregion

            #region data

            private readonly __AVALONIASTORAGEFILE _File;

            #endregion

            #region properties

            public bool IsDirectory => false;            

            public bool Exists => _File != null;

            public long Length => 0;

            public string PhysicalPath => _File.TryGetLocalPath();

            public string Name => _File.Name;

            public DateTimeOffset LastModified { get; }

            #endregion

            #region API

            public Stream CreateReadStream()
            {
                return Task.Run(async () => await _File.OpenReadAsync()).Result;
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("_FolderInfo {Name}")]
        internal class _IStorageFolderAsIFileInfo
            : __XINFO
            , __XCONTENTS
        {
            #region lifecycle
            public _IStorageFolderAsIFileInfo(__AVALONIASTORAGEFOLDER folder)
            {
                _Folder = folder;
                _Contents = new Lazy<List<IFileInfo>>(_ReadContentsBlocking);
                LastModified = DateTime.Now;
            }

            private List<IFileInfo> _ReadContentsBlocking()
            {
                async Task<List<IFileInfo>> _enumerate()
                {
                    var items = new List<IFileInfo>();

                    await foreach (var item in _Folder.GetItemsAsync())
                    {
                        var finfo = ToIFileInfo(item);
                        items.Add(finfo);
                    }

                    return items;
                }

                return Task.Run(async () => await _enumerate()).Result;
            }

            #endregion

            #region data

            private readonly __AVALONIASTORAGEFOLDER _Folder;
            private readonly Lazy<List<IFileInfo>> _Contents;

            #endregion

            #region properties

            public bool IsDirectory => true;            

            public bool Exists => _Folder != null;

            public long Length => 0;

            public string PhysicalPath => _Folder.TryGetLocalPath();

            public string Name => _Folder.Name;

            public DateTimeOffset LastModified { get; }

            #endregion

            #region API

            public Stream CreateReadStream()
            {
                throw new NotSupportedException();
            }

            public IEnumerator<IFileInfo> GetEnumerator()
            {
                return _Contents.Value.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        #endif
    }
}

#endif