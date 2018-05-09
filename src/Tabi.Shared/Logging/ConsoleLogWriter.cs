using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Tabi.Logging
{
    public class ConsoleLogWriter : ILogWriter
    {
        public ConsoleLogWriter() : base()
        { }

        public void Error(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {exception}");
        }

        public void Write(LogSeverity severity, string str)
        {
            System.Diagnostics.Debug.WriteLine($"{severity}: {str}");
        }
    }
}
