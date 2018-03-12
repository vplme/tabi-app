using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Tabi.Logging
{
    public class ConsoleLogWriter : LogWriter
    {
        public ConsoleLogWriter() : base()
        { }

        public override void Write(LogSeverity severity, string str)
        {
#if DEBUG
            base.Write(severity, str);
#endif
        }


        protected override async Task ConsumerAsync(ISourceBlock<string> Source)
        {
            while (await Source.OutputAvailableAsync())
            {
                System.Diagnostics.Debug.WriteLine(Source.Receive());
            }
        }
    }
}
