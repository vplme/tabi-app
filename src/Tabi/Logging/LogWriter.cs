using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Tabi.Logging
{
    public abstract class LogWriter : ILogWriter
    {
        BufferBlock<string> logBufferBlock;
        ITargetBlock<string> logTargetBlock;

        BufferBlock<Exception> errorBufferBlock;
        ITargetBlock<Exception> errorTargetBlock;

        public LogWriter()
        {
            logBufferBlock = new BufferBlock<string>();
            logTargetBlock = logBufferBlock as ITargetBlock<string>;

            errorBufferBlock = new BufferBlock<Exception>();
            errorTargetBlock = errorBufferBlock as ITargetBlock<Exception>;

            LogConsumerAsync(logBufferBlock);
            ErrorConsumerAsync(errorBufferBlock);
        }

        public virtual void Write(LogSeverity severity, string str)
        {
            logTargetBlock.Post(str);
        }

        static void Producer(ITargetBlock<string> Target)
        {
            
        }

        protected abstract Task LogConsumerAsync(ISourceBlock<string> Source);

        protected abstract Task ErrorConsumerAsync(ISourceBlock<Exception> Source);


        public void Error(Exception exception)
        {
            errorTargetBlock.Post(exception);
        }
    }
}
