using System;
namespace Tabi.Logging
{
    public interface ILogWriter
    {
        void Write(LogSeverity severity, string str);
    }
}
