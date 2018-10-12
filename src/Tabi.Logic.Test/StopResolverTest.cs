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
            StopResolverConfig conf = new StopResolverConfig()
            {
                Time = 5,
                GroupRadius = 30,
                MinStopAccuracy = 400,
                StopMergeRadius = 50,
                StopMergeMaxTravelRadius = 200,
            };

            StopResolver stopResolver = new StopResolver(conf);

            PositionEntry p1 = new PositionEntry() { Latitude = 52.083333, Longitude = 5.116667 };
            PositionEntry p2 = new PositionEntry() { Latitude = 52.083333, Longitude = 4.316667 };

            double distance = stopResolver.DistanceBetweenPoints(p1, p2);

            Assert.Equal(54712, Math.Round(distance));
        }

        [Fact]
        public void TestStops()
        {
            var records = LoadPositionsFromCsv("position_entries_public_sample.csv");

            StopResolverConfig conf = new StopResolverConfig()
            {
                Time = 4,
                GroupRadius = 20,
                MinStopAccuracy = 400,
                StopMergeRadius = 50,
                StopMergeMaxTravelRadius = 200,
            };

            StopResolver stopResolver = new StopResolver(conf);

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


            StopResolverConfig conf = new StopResolverConfig()
            {
                Time = 4,
                GroupRadius = 20,
                MinStopAccuracy = 400,
                StopMergeRadius = 50,
                StopMergeMaxTravelRadius = 200,
            };

            StopResolver stopResolver = new StopResolver(conf);

            IList<ResolvedStop> stops = stopResolver.ResolveStops(records);

            foreach (ResolvedStop s in stops)
            {
                Console.WriteLine(s);
            }

            Assert.Equal(0, stops.Count);
        }
    }
}
