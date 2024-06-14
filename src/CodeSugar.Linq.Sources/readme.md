
## CodeSugar overview

This is a collection of commonly used extension methods for `System.Collections.Generic`

## How it works

This package uses some nuget trickery to inject some .cs files directly into your project, without adding any external reference.

The injected files are added by default to the namespace defined by the `<RootNamespace>` of your project.

This behaviour can be changed by declaring this in your project:

```xml
<PropertyGroup>
 <DefineConstants>$(DefineConstants);CODESUGAR_USECODESUGARNAMESPACE</DefineConstants>
</PropertyGroup>
```

which will set the namespace to `CodeSugar.Linq`

## extension methods available


	

