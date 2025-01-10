
## CodeSugar overview

This is a collection of commonly used extension methods for `Microsoft.Extensions.FileProviders.IFileInfo`

[DotNet Runtime Tracking Issues](https://github.com/dotnet/runtime/issues?q=is%3Aissue+is%3Aopen+fileproviders+label%3Aarea-Extensions-FileSystem)

## How it works

This package uses some nuget trickery to inject some .cs files directly into your project, without adding any external reference.

The injected files are added by default to the namespace defined by the `<RootNamespace>` of your project.

This behaviour can be changed by declaring this in your project:

```xml
<PropertyGroup>
 <DefineConstants>$(DefineConstants);CODESUGAR_USECODESUGARNAMESPACE</DefineConstants>
</PropertyGroup>
```

which will set the namespace to `CodeSugar.IO`

## extension methods available


## Alternative file system abstractions

- [zIO](https://github.com/xoofx/zio)
- [SharpCompress](https://github.com/adamhathcock/sharpcompress) not strictly a file system abstraction, but has proper interfaces to do so.
- [TestableIO](https://github.com/TestableIO/System.IO.Abstractions)


## missing APIs proposal:

- [IAsyncDisposable](https://github.com/dotnet/runtime/issues/44673)
- [async methods](https://github.com/dotnet/runtime/issues/37590)
- [flesh out fileprovider (PROPOSAL HERE)](https://github.com/dotnet/runtime/issues/82008)

we need to be able to:
- async operations
- create new directories
- create new files
- write to files
- Parent navigation
- Refresh
- Delete
- Move
- DebuggerDisplay

```c#

interface IParentDirectoryContainer
{
    IDirectoryContents Parent { get; }
}

// meant for read only: https://github.com/dotnet/runtime/issues/82008#issuecomment-1426967449

interface IReadStreamSource
{
    public Stream CreateReadStream(); // overlaps with IFileInfo
    public async Task<Stream> CreateReadStreamAsync();
}

interface IWriteStreamSource
{
    public Stream CreateWriteStream();
    public async Task<Stream> CreateWriteStreamAsync();
}

interface IFileStream
{
    public Stream CreateReadStream(); // overlaps with IFileInfo

    public async Task<Stream> CreateReadStreamAsync();

    public bool IsReadOnly { get; }

    public Stream CreateWriteStream();

    public async Task<Stream> CreateWriteStreamAsync();    

    public void Delete();
}


interface IDirectoryContents
{
    // create a file in memory only, it will be physically created when a write stream is opened
    public bool TryCreateFile(string name, out IFileInfo info) { info = null; return false; }

    // create a directory in memory only, it will be physically created when content is added to it
    public bool TryCreateDirectory(string name, out IFileInfo info) { info = null; return false; }
}
```



	

