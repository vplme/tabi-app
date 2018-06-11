using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Tabi.DataObjects;
using Xunit;

namespace Tabi.Logic.Test
{
    public class StopResolverTest
    {
        private IList<PositionEntry> LoadPositionsFromCsv(string name)
        {
            IList<PositionEntry> records;
            using (StreamReader sr = new StreamReader(Path.Combine("data", name)))
            {
                var csv = new CsvReader(sr);
                csv.Configuration.RegisterClassMap<PositionEntryMap>();
                records = csv.GetRecords<PositionEntry>().ToList();
            }

            return records;
        }

        [Fact]
        public void TestDistanceBetweenPoints()
        {
            StopResolver stopResolver = new StopResolver(TimeSpan.FromMinutes(5), 30, 400, 50, 200);

            PositionEntry p1 = new PositionEntry() { Latitude = 52.083333, Longitude = 5.116667 };
            PositionEntry p2 = new PositionEntry() { Latitude = 52.083333, Longitude = 4.316667 };

            double distance = stopResolver.DistanceBetweenPoints(p1, p2);

            Assert.Equal(54712, Math.Round(distance));
        }

        [Fact]
        public void TestStops()
        {
            var records = LoadPositionsFromCsv("position_entries_public_sample.csv");

            StopResolver stopResolver = new StopResolver(TimeSpan.FromMinutes(4), 20, 400, 50, 200);

            IList<ResolvedStop> stops = stopResolver.ResolveStops(records);

            foreach(ResolvedStop s in stops)
            {
                Console.WriteLine(s);
            }

            Assert.Equal(3, stops.Count);
        }

        [Fact]
        public void TestStopNoStops()
        {
            var records = LoadPositionsFromCsv("position_entries_public_sample.csv");

            StopResolver stopResolver = new StopResolver(TimeSpan.FromHours(4), 20, 400, 50, 200);

            IList<ResolvedStop> stops = stopResolver.ResolveStops(records);

            foreach (ResolvedStop s in stops)
            {
                Console.WriteLine(s);
            }

            Assert.Equal(0, stops.Count);
        }
    }
}
