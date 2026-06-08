
## CodeSugar overview

This is a collection of commonly used extension methods for `System.Numerics.Tensors` and 'SixLabors.ImageSharp'

## CodeSugar.Sources.ICommand overview

`System.Windows.Input.ICommand` interface is universally accepted as the standard for user input action commands.

But it depends on the user to provide the implementation, which in some cases is anything but trivial.

The standard approach is to depend on MVVM packages like [Prism.Core](https://www.nuget.org/packages/Prism.Core)
or [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm), but that introduces a package
dependency that is not always desirable, specially when writing third party libraries where you don't want to 
impose a certain dependency on the developer.

This package injects `ICommand` implementations directly into the code without any requiring any dependency.

## extension methods available

Given a method, we can create an ICommand like this:

```csharp
class Foo
{
	public ICommand SomeCommand => ((Action)SomeAction1).ToInputICommand();

    private void SomeAction1() { }

	public ICommand SomeCommand2 => ((Action<int>)SomeAction).ToInputICommand();

    private void SomeAction2(int arg) { }
}
```






	

