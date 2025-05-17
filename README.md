# CodeSugar

This repository contain projects that add extensions for system classes, like System.IO.

The extensions are consumed by referencing the nuget packages, but the packages only inject the actual source code to your projects.

because the source files are directly injected into your project, all classes must be defined as _internal_, which prevents naming and overlapping conflicts downstream.

### Why sources instead of regular package references?

Consuming packages as source code has advantages and disadvantages:

The purpose of these packages is to pack many sparse but commonly used extension methods that can be easily consumed.

Advantages

- Reuse tiny bits of code here and there.
- Don't bloat your project with external dependencies that might create version conflicts downstream.
- Conditional Debug/Release support.
- Namespace of the extension classes are automatically adjusted to the namespace of your project, which makes the extensions easier to use.

Disadvantages

- Since there's no external package reference, and the classes are declared as internal, the code is not propagated transitively, so you need to reference these packages on _every_ project you want the extensions to be available.
- It might fail if your project's path is very long due to how this kind of packages is consumed.

### Source code injection considerations

There's different ways of adding source code straight into projects. One is this method of using Content Only nuget packages. The other one ise using Source Generator projects.

I've chosen Content Only packages since it seems a more straightforward solution.

### Instructions

These packages are consumed in a similar way than an analyser:

```xml
 <PropertyGroup>
   <!-- for some reason, some projects require RootNamespace to be defined, in order for the templates to be applied. -->
   <RootNamespace>YourProjectNamespace</RootNamespace> 
 </PropertyGroup>

 <ItemGroup>
    <!-- PrivateAssets="all" prevents your project from depending on CodeSugar packages, in the same way analyzers do. -->
    <PackageReference Include="CodeSugar.System.IO.Sources" Version="1.0.0-Preview-20240105-084731" PrivateAssets="all" />
  </ItemGroup>
```

Immediately after adding the package, it's recomended to build the project for Visual Studio intellisense to notice about the newly added files.

Afterwards, the extensions should be readily available:

```c#
    public void ExampleOfStreamWrite()
    {
        using(var m = new System.IO.MemoryStream())
        {
            m.WriteAllBytes(new byte[1024]);
            m.WriteValue<int>(10);
            m.WriteValue<float>(0.1f);
            m.WriteString("hello world");
        }
    }
```

### Known MSBuild issues

1. Projects referencing a source package with templates requires to have a &lt;RootNamespace&gt; defined in the csproj, even if it
matches the default value, otherwise msbuild is unable to generate the templates.

2. Apparently, REbuilding a project containing templated source files does not work, only Build works.

3. The nuget templating system copies the .cs generated code into the Obj directory, in a fairly long path, which looks like this:

```C:\Users\CurrentUser\Desktop\MyProjects\CurrentSolution\Src\SomeProject\obj\Debug\net8.0\NuGet\335213469D559D6E09240088A99B3E9B5ADBC7AC\CodeSugar.System.IO.Sources\1.0.0-Preview-20240119-092135\_CodeSugarIO.DriveInfo.cs```

Which can exceed the maximum number of characters supported by a file path, which will result in a ```MSB4018	The "ProduceContentAssets" task failed unexpectedly.``` Error.

[NuGet and long file name support](https://github.com/NuGet/Home/issues/3324)

4. if two projects that depend one upon the other, and also define `InternalsVisibleTo` , then only the parent
project must reference a given CodeSugar package, because since CodeSugar extensions are stored in internal
classes, they're made visible to other projects.


### External references

- [Content Files Nuget Example](https://www.nuget.org/packages/ContentFilesExample)
  - [github](https://github.com/NuGet/Samples/tree/main/ContentFilesExample)
  - [nuget issue](https://github.com/NuGet/Home/issues/7919)





