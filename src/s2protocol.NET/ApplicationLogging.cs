using Microsoft.Extensions.Logging;

namespace s2protocol.NET;

internal static class ApplicationLogging
{
    private static readonly Action<ILogger, string, Exception?> _engineStarted = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(2, nameof(EngineStarted)),
        "Engine start (Engine = '{EngineStarted}')");

    public static void EngineStarted(this ILogger logger, string engineString)
    {
        _engineStarted(logger, engineString, null);
    }

    private static readonly Action<ILogger, string, Exception?> _engineError = LoggerMessage.Define<string>(
        LogLevel.Error,
        new EventId(99, nameof(EngineError)),
        "Engine error (Error = '{EngineError}')");

    public static void EngineError(this ILogger logger, string engineError)
    {
        _engineError(logger, engineError, null);
    }

    private static readonly Action<ILogger, string, Exception?> _decodeError = LoggerMessage.Define<string>(
        LogLevel.Error,
        new EventId(98, nameof(DecodeError)),
        "decode error (Error = '{DecodeError}')");

    public static void DecodeError(this ILogger logger, string engineError)
    {
        _decodeError(logger, engineError, null);
    }

    private static readonly Action<ILogger, string, Exception?> _decodeWarning = LoggerMessage.Define<string>(
        LogLevel.Warning,
        new EventId(57, nameof(DecodeWarning)),
        "decode warning (Warning = '{DecodeWarning}')");
    public static void DecodeWarning(this ILogger logger, string engineWarning)
    {
        _decodeWarning(logger, engineWarning, null);
    }

    private static readonly Action<ILogger, string, Exception?> _decodeInformation = LoggerMessage.Define<string>(
        LogLevel.Information,
        new EventId(57, nameof(DecodeInformation)),
        "decode information (Information = '{DecodeInformation}')");
    public static void DecodeInformation(this ILogger logger, string engineInformation)
    {
        _decodeInformation(logger, engineInformation, null);
    }

    private static readonly Action<ILogger, string, Exception?> _decodeDebug = LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(57, nameof(DecodeDebug)),
        "decode debug (Debug = '{DecodeDebug}')");
    public static void DecodeDebug(this ILogger logger, string engineDebug)
    {
        _decodeDebug(logger, engineDebug, null);
    }
}