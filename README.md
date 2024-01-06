# CodeSugar

This repository contain projects that add extensions for system classes, like System.IO.

The extensions are consumed by referencing the nuget packages, but the packages only inject the actual source code to your projects.

because the source files are directly injected into your project, all classes must be defined as _internal_, which prevents naming and overlapping conflicts downstream.

### Why sources instead of regular package references?

Consuming packages as source code has advantages and disadvantages:

The purpose of these packages is to pack many sparse but commonly used extension methods that can be easily consumed.

This is ideal when you want to reuse tiny bits of code here and there, but you don't want to bloat your project with external dependencies that might create version conflicts downstream.

Another advantage is that the namespace of the extension classes are automatically adjusted to the namespace of your project, which makes the extensions easier to use.

The disadvantage is that, since there's no external package reference, and the classes are declared as internal, the code is not propagated transitively, so you need to reference these packages on _every_ project you want the extensions to be available.





