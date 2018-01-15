using System;
using System.Diagnostics;
using MacroSystem;


namespace
MacroConsole
{


/// <summary>
/// A <see cref="TraceListener"/> that writes <see cref="Trace"/> output to <see cref="System.Console.Error"/> in a
/// format suitable for use by console programs to display informational output
/// </summary>
///
/// <remarks>
/// This approach has the added benefit of including informational output from libraries with no additional mechanisms
/// or work required.
/// </remarks>
///
/// <example>
/// <code>
/// //
/// // Attach a ConsoleApplicationTraceListener on program startup
/// //
/// Trace.Listeners.Add(new ConsoleApplicationTraceListener());
///
/// //
/// // ...and then later, write output using the standard Trace methods
/// //
/// Trace.TraceInformation("This will be written to System.Console.Error as-is");
/// Trace.TraceWarning("This will be written to System.Console.Error prefixed by [Warning]");
/// Trace.TraceError("This will be written to System.Console.Error prefixed by [Error]");
/// </code>
/// </example>
///
public class
ConsoleApplicationTraceListener
    : TraceListener
{


public
ConsoleApplicationTraceListener()
{
}


public override void
WriteLine(string message)
{
   Console.Error.WriteLine(message);
}


public override void
Write(string message)
{
    Console.Error.Write(message);
}


/// <inheritdoc/>
///
public override void
TraceEvent(
    TraceEventCache eventCache,
    string source,
    TraceEventType eventType,
    int id,
    string format,
    params object[] args)
{
    TraceEvent(eventCache, source, eventType, id, StringExtensions.FormatInvariant(format, args));
}


/// <inheritdoc/>
///
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Globalization",
    "CA1308:NormalizeStringsToUppercase",
    Justification = "Actually want lowercase")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Globalization",
    "CA1303:Do not pass literals as localized parameters",
    MessageId = "System.Diagnostics.TraceListener.WriteLine(System.String)",
    Justification = "English-only for now")]
public override void
TraceEvent(
    TraceEventCache eventCache,
    string source,
    TraceEventType eventType,
    int id,
    string message)
{ 
    message = message ?? "";
    var prefix = "";

    switch (eventType)
    {
        case TraceEventType.Information:
        case TraceEventType.Verbose:
            break;
        case TraceEventType.Stop:
            message = "Finished " + message.Substring(0, 1).ToLowerInvariant() + message.Substring(1);
            goto case TraceEventType.Start;
        case TraceEventType.Start:
            prefix = LogicalOperationPrefix(eventCache);
            break;
        default:
            prefix = EventTypePrefix(eventType);
            break;
    }

    WriteLine(prefix + message);
}


static string EventTypePrefix(TraceEventType eventType)
{
    return "[" + eventType.ToString() + "] ";
}


static string LogicalOperationPrefix(TraceEventCache eventCache)
{
    return new string('-', eventCache.LogicalOperationStack.Count) + "> ";
}


}
}
