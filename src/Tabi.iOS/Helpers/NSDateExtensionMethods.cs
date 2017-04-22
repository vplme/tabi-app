using System;
using Foundation;

namespace Tabi.iOS.Helpers
{
    public static class NSDateExtensionMethods
    {
		static DateTime reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime ToDateTime(this NSDate date)
		{
			var dateTime = reference.AddSeconds(date.SecondsSinceReferenceDate);
			return dateTime;
		}

		public static NSDate ToNSDate(this DateTime datetime)
		{
			var utcDateTime = datetime.ToUniversalTime();
			var date = NSDate.FromTimeIntervalSinceReferenceDate((utcDateTime - reference).TotalSeconds);
			return date;
		}
    }
}
