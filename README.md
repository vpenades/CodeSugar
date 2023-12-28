# CodeSugar

This repository contain projects that add extensions for system classes, like System.IO.

The extensions are consumed by referencing the nuget packages, but the packages only inject the actual source code to your projects.

Consuming packages as source code has advantages and disadvantages:

The main advantage is that your project does not drag any third party dependency, since the code is compiled as part of your project.

Another advantage is that the namespace of the extension is automatically adjusted to the namespace of your project, which makes the extensions easier to use.

The disadvantage is that, since there's no external reference, the code is not propagated, so you need to reference these packages on _every_ project you want the extensions to be available.





