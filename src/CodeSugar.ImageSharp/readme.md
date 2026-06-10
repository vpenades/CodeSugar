
## CodeSugar overview

This is a collection of commonly used extension methods for `System.Numerics.Tensors` and 'SixLabors.ImageSharp'

## CodeSugar.Sources.ImageSharp overview

This source injection provides a collection of boiler plate extensions for SixLabors.ImageSharp (up to version 3.1.12)

## Added functionality:

Random pixel sampling:

```csharp
var sampler = image.GetScaledVector4Sampler(true);
var sample = sampler[new Vector2(0.5f, 0.7f)];
```
Interop with System.Numerics.Tensors

```csharp

// image to tensor
var tensor = new System.Numerics.Tensors.TensorSpan<float>(...);
image.CopyToTensor(tensor);

// tensor to image
tensor.CopyToSixLaborsImage(image);


```







	

