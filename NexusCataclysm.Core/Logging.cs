using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using System.Text;

namespace NexusCataclysm.Core;
public static class Logging
{
    /// <summary>
    /// Return a default configuration with suitable value.
    /// </summary>
    /// <param name="enableRegexColored">是否启用正则表达式进行着色，这会导致性能降低，但是输出将会变得beautiful</param>
    /// <returns>default configuration</returns>
    public static LoggingConfiguration CreateDefaultConfiguration()
    {
        var config = new LoggingConfiguration();

        config.AddRule(LogLevel.Trace, LogLevel.Fatal, SetupConsoleTarget());
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, SetupFileTarget());

        return config;
    }

    /// <summary>
    /// Return a default options with suitable value.
    /// </summary>
    /// <returns>default options</returns>
    public static NLogProviderOptions CreateDefaultOptions()
    {
        var options = new NLogProviderOptions
        {
            AutoShutdown = false,
            ShutdownOnDispose = true,
        };
        return options;
    }

    private static Target SetupFileTarget()
    {
        var logfile = new FileTarget("logfile")
        {
            FileName = "Log/Current.log",
            LineEnding = LineEndingMode.LF,
            Encoding = Encoding.UTF8,
            ArchiveFileName = "Log/Archived.{###}.log",
            ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
            ArchiveDateFormat = "yyyy.MM.dd",
            MaxArchiveFiles = 128,
            ArchiveOldFileOnStartup = true,
            EnableArchiveFileCompression = true,
            ArchiveEvery = FileArchivePeriod.Day,
        };

        logfile.Layout = new JsonLayout()
        {
            Attributes =
            {
                new JsonAttribute("time", "${longdate}"),
                new JsonAttribute("level", "${level}"),
                new JsonAttribute("thread","${threadname}"),
                new JsonAttribute("logger","${logger}"),
                new JsonAttribute("raw-message","${message:raw=true}"),
                new JsonAttribute("message", "${message}"),
                new JsonAttribute("properties", new JsonLayout { IncludeEventProperties = true, MaxRecursionLimit = 8 }, encode: false),
                new JsonAttribute("exception", new JsonLayout
                {
                    Attributes =
                    {
                        new JsonAttribute("callsite","${callsite}"),
                        new JsonAttribute("type", "${exception:format=type}"),
                        new JsonAttribute("message", "${exception:format=message}"),
                        new JsonAttribute("stacktrace", "${exception:format=tostring}"),
                    },
                },
                encode: false), // don't escape layout
            },
            IndentJson = true,
        };

        return logfile;
    }

    private static Target SetupConsoleTarget()
    {
        var console = new ColoredConsoleTarget("logconsole")
        {
            Encoding = Encoding.UTF8,
            Layout = @"[${longdate}][${level}][${threadname}::${logger}]:${message}${onexception:inner=${newline}${exception}}",
        };

        console.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("true", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
            { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("false", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
            { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("start", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
            { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("begin", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
            { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("stop", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
            { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
            new ConsoleWordHighlightingRule("cancel", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
            { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
           new ConsoleWordHighlightingRule("trigger", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
           { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
           new ConsoleWordHighlightingRule("without", ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange)
           { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
           new ConsoleWordHighlightingRule("not", ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange)
           { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
           new ConsoleWordHighlightingRule("SDL", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange)
           { IgnoreCase = false, WholeWords = false });
        console.WordHighlightingRules.Add(
           new ConsoleWordHighlightingRule("call", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
           { IgnoreCase = true, WholeWords = true });
        console.WordHighlightingRules.Add(
           new ConsoleWordHighlightingRule("invoke", ConsoleOutputColor.DarkBlue, ConsoleOutputColor.NoChange)
           { IgnoreCase = true, WholeWords = true });

        return console;
    }
}
