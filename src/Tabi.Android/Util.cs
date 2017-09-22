using System;
namespace Tabi.Droid
{
    public static class Util
    {
        public static DateTime TimeLongToDateTime(long time)
        {
			DateTime reference = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return reference.AddMilliseconds(time);
		}
    }
}
