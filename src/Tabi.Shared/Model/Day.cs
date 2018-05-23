using System;
using System.Linq;
using Tabi.Shared.Resx;

namespace Tabi.Model
{
    public class Day
    {
        public DateTime Time { get; set; }

        public string CurrentDate
        {
            get
            {
                if (Time.Date == DateTime.Today)
                {
                    return AppResources.Today;
                }
                else if (Time.Date == DateTime.Today.Subtract(TimeSpan.FromDays(1)))
                {
                    return AppResources.Yesterday;
                }

                return ToUpper(Time.ToString("dddd d MMMM yyyy"));
            }
        }

        public string CurrentDateShort
        {
            get
            {
                if (Time.Date == DateTime.Today)
                {
                    return AppResources.Today;
                }
                else if (Time.Date == DateTime.Today.Subtract(TimeSpan.FromDays(1)))
                {
                    return AppResources.Yesterday;
                }

                return ToUpper(Time.ToString("ddd d MMM"));

            }
        }


        private string ToUpper(string str)
        {
            string result = str;

            if (!string.IsNullOrEmpty(str))
            {
                result = str.First().ToString().ToUpper() + str.Substring(1);
            }
            return result;
        }
    }
}
