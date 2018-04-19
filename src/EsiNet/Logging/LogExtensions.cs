using System;

namespace EsiNet.Logging
{
    public static class LogExtensions
    {
        public static void Debug(this Log log, Func<string> message)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (message == null) throw new ArgumentNullException(nameof(message));
            log(LogLevel.Debug, null, message);
        }

        public static void Debug(this Log log, Func<string> message, Exception exception)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (message == null) throw new ArgumentNullException(nameof(message));
            log(LogLevel.Debug, exception, message);
        }

        public static void Information(this Log log, Func<string> message)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (message == null) throw new ArgumentNullException(nameof(message));
            log(LogLevel.Information, null, message);
        }

        public static void Information(this Log log, Func<string> message, Exception exception)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (message == null) throw new ArgumentNullException(nameof(message));
            log(LogLevel.Information, exception, message);
        }

        public static void Warning(this Log log, Func<string> message)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (message == null) throw new ArgumentNullException(nameof(message));
            log(LogLevel.Warning, null, message);
        }

        public static void Warning(this Log log, Func<string> message, Exception exception)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (message == null) throw new ArgumentNullException(nameof(message));
            log(LogLevel.Warning, exception, message);
        }

        public static void Error(this Log log, Func<string> message)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (message == null) throw new ArgumentNullException(nameof(message));
            log(LogLevel.Error, null, message);
        }

        public static void Error(this Log log, Func<string> message, Exception exception)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (message == null) throw new ArgumentNullException(nameof(message));
            log(LogLevel.Error, exception, message);
        }
    }
}