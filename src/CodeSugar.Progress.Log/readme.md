
## CodeSugar overview

This is a collection of commonly used extension methods over various areas **using source code injection packages instead of actual dependencies**.

## Progress Logging

There's a plethora of logging frameworks, all of them requiring your projects to depend on them. This prevents
libraries that don't want to drag any third party dependency to implement any kind of minimally interoperable
logging system.

This package addresses this by:

- Leveraging `System.IProgress<string>` and `System.Diagnostics.TraceEventType` as the foundation for the logging system.
- Adding source code only extensions with well known logging methods.
- Default sink writing to console, which can be redirected to a file.

### Repurposing TraceEventType 

TraceEventType has pretty much everything needed to declare logging levels:

|TraceEventType|Ms Logging|
|-|-|
|Verbose|Debug|
|Verbose|Trace|
|Information|Information|
|Warning|Warning|
|Error|Error|
|Critical|Critical|
|Start|Begin Scope|
|Stop|End Scope|
|Suspend| - |
|Resume| - |
|Transfer| - |

### Usage example

Windows Desktop redirect:

```c#
class Program
{
    private static readonly IProgress<string> log = typeof(Program).GetProgressToConsoleLogger();

    static void Main(string[] args)
    {
        // Create a StreamWriter to write to the file
        using (StreamWriter writer = new StreamWriter("console_output.txt"))
        {
            // Set the StreamWriter as the Console output
            Console.SetOut(writer);

            // Write to the log (this will go to the file instead)
            log.LogInformation("Hello from file!");            
        }

        // After the using block, Console output will revert to the default
        log.LogInformation("Hello from console!");
    }
}
```

Android redirect:

```c#
using System;
using System.IO;
using Android.Util;

public class LogRedirect : TextWriter
{
    private readonly string _tag;
    private readonly LogPriority _logPriority;

    public LogRedirect(string tag, LogPriority logPriority = LogPriority.Info)
    {
        _tag = tag;
        _logPriority = logPriority;
    }

    public override void WriteLine(string value)
    {
        Log.WriteLine(_logPriority, _tag, value);
    }

    public override void Write(char value)
    {
        // Optional: Buffer single characters, or ignore if not needed.
    }

    public override Encoding Encoding => Encoding.UTF8;
}

// Usage in MainActivity.cs or similar
protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnCreate(savedInstanceState);

    // Redirect Console output
    Console.SetOut(new LogRedirect("MyAppTag", LogPriority.Info));
    Console.SetError(new LogRedirect("MyAppTag", LogPriority.Error));

    // Test logging

    IProgress<string> log = typeof(Program).GetProgressToConsoleLogger();

    log.LogInformation("Hello from file!");      
}

```



### Dependency injection

This library does not provide any shared repository for dependency injection, Suggested solutions are:

- use Microsoft.Extensions.DependencyInjection
- Pass a factory lambda on each constructor
- Static Factory, either shared or defined on each library:

```c#
public static ProgressLoggingManager
{
    private static Func<Type,IProgres<string>> _LogFactory;

    public IProgress<string> GetLogger(Type type)
    {
        return _LogFactory.Invoke(type);
    }
}

```

### To Do:

Console has .Error as long as .Out , console sink may be able to map to which console output we should
redirect the errors.

Microsoft Logging supports string interpolation [with names](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#log-message-template), as in:

```c#
__LOGGER.LogInformation("Getting item {Id} at {RunTime}", id, DateTime.Now);
```

classed involved seem to be:
- [LogValuesFormatter.cs](https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Logging.Abstractions/src/LogValuesFormatter.cs)
- [FormattedLogValues.cs](https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Logging.Abstractions/src/FormattedLogValues.cs)



