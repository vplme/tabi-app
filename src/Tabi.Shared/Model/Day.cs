using System;
namespace Tabi.Model
{
    public class Day
    {
        public DateTime Time { get; set; }

        public string CurrentDate {
            get {
                return Time.ToString("dddd d MMMM, yyyy");
            }
        }
    }
}
