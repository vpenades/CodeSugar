
## CodeSugar overview

This is a collection of commonly used extension methods for `System.IO.FileInfo` and `System.IO.DirectoryInfo`

## How it works

This package uses some nuget trickery to inject some .cs files directly into your project, without adding any external reference.

The injected files are added by default to the namespace defined by the `<RootNamespace>` of your project.

## extension methods available

- Stream
  - `string ReadAllText();`
  - `void WriteAllText(string text);`
  - `ArraySegment<Byte> ReadAllBytes();`
  - `void WriteAllBytes(IReadOnlyList<Byte> bytes);`
  - And more...

- DirectoryInfo
  - `FileInfo GetFile(params string[] name);`
  - `FileInfo UseFile(params string[] name);`
  - `DirectoryInfo GetDirectory(params string[] name);`	
  - `DirectoryInfo UseDirectory(params string[] name);`	
  - And more...
	

