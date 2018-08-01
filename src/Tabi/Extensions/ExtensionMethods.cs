using System;
namespace Tabi.Extensions
{
    public static class DateTimeHelper
    {
        public static DateTimeOffset FromUnixTime(long unixTime)
        {
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromMilliseconds((unixTime)));
        }

        public static long ToUnixTime(this DateTimeOffset dto)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((dto - epoch).TotalSeconds);
        }
    }
}
