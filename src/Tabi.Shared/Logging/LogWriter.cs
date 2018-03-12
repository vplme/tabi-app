using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Tabi.Logging
{
    public abstract class LogWriter : ILogWriter
    {
        BufferBlock<string> bufferBlock;
        ITargetBlock<string> target;

        public LogWriter()
        {
            bufferBlock = new BufferBlock<string>();
            target = bufferBlock as ITargetBlock<string>;
            ConsumerAsync(bufferBlock);
        }

        public virtual void Write(LogSeverity severity, string str)
        {
            target.Post(str);
        }

        static void Producer(ITargetBlock<string> Target)
        {
            
        }

        protected abstract Task ConsumerAsync(ISourceBlock<string> Source);
    }
}
