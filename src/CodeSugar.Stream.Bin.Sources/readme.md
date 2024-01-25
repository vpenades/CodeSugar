
## CodeSugar overview

Injects binary serialization extension sources for `System.IO.Stream`.

### Extension methods

```c#
public static bool TryReadBytes(this Stream stream, Span<Byte> bytes)
public static T ReadValue<T>(this Stream stream, bool streamIsBigEndian = false)
public static void WriteValue<T>(this Stream stream, T value, bool streamIsBigEndian = false)
public static void WriteSigned64Packed(this Stream stream, long value)
public static void WriteUnsigned64Packed(this Stream stream, ulong uValue)
public static long ReadSigned64Packed(this Stream stream)
public static ulong ReadUnsigned64Packed(this Stream stream)
public static void WriteString(this Stream stream, string text, System.Text.Encoding encoding = null)
public static string ReadString(this Stream stream, System.Text.Encoding encoding = null)
```
