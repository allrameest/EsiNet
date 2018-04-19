using System;

namespace EsiNet.Logging
{
    public delegate void Log(LogLevel logLevel, Exception exception, Func<string> message);
}