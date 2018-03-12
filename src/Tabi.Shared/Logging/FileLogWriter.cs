﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using PCLStorage;
using Tabi.Helpers;

namespace Tabi.Logging
{
    public class FileLogWriter : LogWriter
    {
        public static EventHandler<EventArgs> LogWriteEvent;
        LogFileAccess logAccess;

        public FileLogWriter() : base()
        {
            logAccess = new LogFileAccess();
        }

        private async Task WriteToFile(string text)
        {
            await logAccess.AppendAsync(text);
            LogWriteEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override async Task ConsumerAsync(ISourceBlock<string> Source)
        {
            while (await Source.OutputAvailableAsync())
            {
                await WriteToFile($"{DateTime.Now} {Source.Receive()}\n");
                LogWriteEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
