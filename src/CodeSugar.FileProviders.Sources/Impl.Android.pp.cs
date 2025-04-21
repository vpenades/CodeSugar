// Copyright (c) CodeSugar 2024 Vicente Penades

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Linq;


#if ANDROID

using _ANDROIDCONTEXT = Android.Content.ContextWrapper;
using _ANDROIDASSETS = Android.Content.Res.AssetManager;

using _XINFO = Microsoft.Extensions.FileProviders.IFileInfo;
using _XPROVIDER = Microsoft.Extensions.FileProviders.IFileProvider;
using _XDIRECTORY = Microsoft.Extensions.FileProviders.IDirectoryContents;

#nullable disable

#if CODESUGAR_USECODESUGARNAMESPACE
namespace CodeSugar
#elif CODESUGAR_USESYSTEMNAMESPACE
namespace System.IO
#else
namespace $rootnamespace$
#endif
{
    internal static partial class CodeSugarForFileProviders
    {
        #region API

        public static _XPROVIDER CreateAndroidAssetManagerFileProvider(this _ANDROIDCONTEXT context)
        {
            return _AssetManagerFileProvider.Create(context);
        }

        public static _XPROVIDER CreateAndroidAssetManagerFileProvider(this _ANDROIDASSETS assets)
        {
            return _AssetManagerFileProvider.Create(assets);
        }

        #endregion

        #region nested types

        /// <summary>
        /// exposes the files contained in <see cref="_ANDROIDASSETS"/> as a cross platform <see cref="_XPROVIDER"/> tree.
        /// </summary>
        sealed class _AssetManagerFileProvider : _XPROVIDER
        {
            #region lifecycle

            public static _AssetManagerFileProvider Create(_ANDROIDCONTEXT context)
            {
                return Create(context?.Assets);
            }

            public static _AssetManagerFileProvider Create(_ANDROIDASSETS assets)
            {                
                if (assets == null) return null;
                return new _AssetManagerFileProvider(assets);
            }

            private _AssetManagerFileProvider(_ANDROIDASSETS assets)
            {
                if (assets == null) throw new ArgumentNullException(nameof(assets));

                _Assets = assets;
                
                _Root = _AndroidAsset.CreateFrom(assets, string.Empty) as _XDIRECTORY;
            }            

            #endregion

            #region data            

            private readonly _ANDROIDASSETS _Assets;

            private readonly _XDIRECTORY _Root;            

            #endregion

            #region API

            public _XDIRECTORY GetDirectoryContents(string subpath)
            {
                if (string.IsNullOrWhiteSpace(subpath)) return _Root;

                return GetFileInfo(subpath)
                    as _XDIRECTORY
                    ?? Microsoft.Extensions.FileProviders.NotFoundDirectoryContents.Singleton;
            }

            public _XINFO GetFileInfo(string subpath)
            {
                subpath = subpath.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
                subpath = subpath.Trim(System.IO.Path.AltDirectorySeparatorChar);

                if (string.IsNullOrEmpty(subpath)) return _Root as _XINFO;

                var parts = subpath.Split('/');

                var entry = __FindEntry(_Root, StringComparison.Ordinal, parts);
                if (entry is null) return new Microsoft.Extensions.FileProviders.NotFoundFileInfo(subpath);
                return entry;
            }            

            public IChangeToken Watch(string filter)
            {
                throw new NotImplementedException();
            }           

            #endregion
        }

        abstract class _AndroidAsset : IServiceProvider
        {
            #region lifecycle

            public static _AndroidAsset CreateFrom(_ANDROIDASSETS assets, string path)
            {
                var entries = assets.List(path) ?? Array.Empty<string>();

                if (!string.IsNullOrEmpty(path))
                {
                    if (entries == null || entries.Length == 0)
                    {
                        return _AndroidAssetFile.Create(assets, path);
                    }
                    path += "/";
                }

                var contents = entries
                    .Select(entry => CreateFrom(assets, path + entry))
                    .Where(item => item != null)
                .ToArray();

                return new _AndroidAssetDirectory(path, contents);
            }

            public _AndroidAsset(string path)
            {
                path = path.Replace(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
                path = path.Trim(System.IO.Path.AltDirectorySeparatorChar);

                AssetFullPath = path;
                Name = System.IO.Path.GetFileName(path);
            }

            #endregion

            #region data

            public _AndroidAssetDirectory Parent { get; set; }

            public string AssetFullPath { get; }

            public string? PhysicalPath => null;

            public string Name { get; }

            public bool Exists => true;

            #endregion

            #region API

            public virtual object GetService(Type serviceType)
            {
                if (serviceType == typeof(System.IO.MatchCasing)) return System.IO.MatchCasing.CaseSensitive;
                return null;
            }

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("🗎 {Name}")]
        sealed class _AndroidAssetFile : _AndroidAsset, _XINFO
        {
            #region lifecycle

            public static _AndroidAssetFile Create(_ANDROIDASSETS assets, string path)
            {
                try
                {
                    using (var afd = assets.OpenFd(path))
                    {
                        // what's the difference between afd.Length && afd.DeclaredLength ?
                        return new _AndroidAssetFile(assets, path, afd.Length);
                    }
                }
                catch (Java.IO.FileNotFoundException) // this happended on an Office's Galaxy tablet
                {
                    // [0:] Error trying to open Asset: geoid_height_map / README.md
                    // [0:] Error trying to open Asset: geoid_height_map / map -params.pb
                    System.Diagnostics.Trace.WriteLine($"Error trying to open Asset: {path}");
                    return null;
                }
                catch (Java.IO.IOException)
                {
                    System.Diagnostics.Trace.WriteLine($"Error trying to open Asset: {path}");
                    return null;
                }
            }

            private _AndroidAssetFile(_ANDROIDASSETS assets, string path, long len) : base(path)
            {
                this._Assets = assets;
                this.Length = len;
            }

            #endregion

            #region data

            private readonly _ANDROIDASSETS _Assets;

            #endregion

            #region properties            

            public long Length { get; }

            public DateTimeOffset LastModified { get; }
            public bool IsDirectory => false;

            #endregion

            #region API

            public Stream CreateReadStream() { return _Assets.Open(AssetFullPath); }            

            #endregion
        }

        [System.Diagnostics.DebuggerDisplay("📁 {Name}")]
        sealed class _AndroidAssetDirectory : _AndroidAsset, _XINFO, _XDIRECTORY
        {
            #region lifecycle

            public _AndroidAssetDirectory(string path, _AndroidAsset[] contents) : base(path)
            {
                _Contents = contents;

                foreach (var c in contents) c.Parent = this;
            }

            #endregion

            #region data

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
            private readonly IReadOnlyList<_AndroidAsset> _Contents;

            #endregion

            #region properties
            public long Length => -1;
            public DateTimeOffset LastModified { get; } = DateTimeOffset.Now.Date;
            public bool IsDirectory => true;

            #endregion

            #region API
            public Stream CreateReadStream() { throw new NotSupportedException(); }
            public IEnumerator<_XINFO> GetEnumerator() => _Contents.Cast<_XINFO>().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() { return _Contents.GetEnumerator(); }

            #endregion
        }

        #endregion
    }
}

#endif
