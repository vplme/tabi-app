using System;
namespace Tabi.Logging
{
    public interface ILogWriter
    {
        void Write(string str);
    }
}
