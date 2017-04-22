using System;
using Foundation;

namespace Tabi.iOS.Helpers
{
    public static class Utils
    {
        public static DateTimeOffset TimeSinceBootToDateTimeOffset(double uptime)
        {
			double boot = NSProcessInfo.ProcessInfo.SystemUptime;
            DateTimeOffset bootTimestamp = DateTimeOffset.Now.AddSeconds(-boot);
            return bootTimestamp.AddSeconds(uptime); 
        }
    }
}
