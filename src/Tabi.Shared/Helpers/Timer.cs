using System;
namespace Tabi.Shared.Helpers
{
    public class Timer
    {
        private DateTime StartTime;
        private DateTime EndTime;

        public void Start()
        {
            StartTime = DateTime.Now;
        }


        public void End()
        {
            EndTime = DateTime.Now;
        }

        public TimeSpan EndAndReturnTime()
        {
            End();

            return Duration;
        }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan result = TimeSpan.Zero;

                result = EndTime.Subtract(StartTime);

                return result;
            }
        }
    }
}
