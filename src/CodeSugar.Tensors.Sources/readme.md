
## CodeSugar overview

This is a collection of commonly used extension methods for `System.Numerics.Tensors`

## CodeSugar.Sources.Tensors overview

This source injection provides a collection of boiler plate extensions for the new `System.Numerics.Tensors.TensorSpan<T>`

Copy pixels between spans:

```csharp
var src = new byte[768];
var dst = new Vector3[256];

src.AsReadOnlySpan().ConvertRGBToRGB(dst);

```


	

