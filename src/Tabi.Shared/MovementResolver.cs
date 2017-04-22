using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tabi.Model;

namespace Tabi
{
    public class MovementResolver
    {
        Repository<AccelerometerData> repository;
        public MovementResolver()
        {
            //repository = new Repository<AccelerometerData>(SQLiteHelper.Instance.Connection);
        }

        private void ProcessDataAsync(DateTimeOffset start, DateTimeOffset end, TimeSpan interval)
        {
            List<Movement> movements = new List<Movement>();
            DateTimeOffset currentTime = start;
            while (start >= end)
            {
                List<AccelerometerData> data = GetDataForPeriodAsync(currentTime, interval).Result;
                bool result = ResolveMovementSegment(data);

                Movement m = new Movement()
                {
                    startTimestamp = currentTime,
                    endTimestamp = currentTime.Add(interval),
                    Moved = result,
                };

                movements.Add(m);
                currentTime.Add(interval);
            };
        }


        private async Task<List<AccelerometerData>> GetDataForPeriodAsync(DateTimeOffset start, TimeSpan span)
        {
            //repository.Get((x) => { x.Timestamp >= start && x.Timestamp <= x.Timestamp.Add(span); }, null);
            List<AccelerometerData> data = await repository.AsQueryable().Where(x => x.Timestamp >= start && x.Timestamp <= x.Timestamp.Add(span)).ToListAsync();
            return data;
        }

        private bool ResolveMovementSegment(List<AccelerometerData> data)
        {
            //bool movement = false;
            //foreach (AccelerometerData d in data)
            //{

            //}
            return true;
        }
    }

    public class Movement
    {
        public DateTimeOffset startTimestamp { get; set; }
        public DateTimeOffset endTimestamp { get; set; }
        public bool Moved { get; set; }

    }

    public class MovementRegistry
    {
        public DateTimeOffset startTimestamp { get; set; }
        public DateTimeOffset endTimestamp { get; set; }
        public bool Moved { get; set; }

    }
}
