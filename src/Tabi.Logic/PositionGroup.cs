using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tabi.DataObjects;

namespace Tabi.Logic
{
    public class PositionGroup : IEnumerable<PositionEntry>
    {
        public List<PositionEntry> Positions { get; set; } = new List<PositionEntry>();

        public PositionGroup()
        {
        }


        public void Add(PositionEntry entry)
        {
            Positions.Add(entry);
        }

        public TimeSpan TimeSpent => Positions.Last().Timestamp - Positions.First().Timestamp;

        public double AverageAccuracy => Positions.Average(p => p.Accuracy);

        public double WeightedAverage
        {
            get
            {
                return Positions.Sum(p => (p.Accuracy < 80 ? 1 : 0.25) * p.Accuracy) / Positions.Count;
            }
        }

        public double MaxAccuracy => Positions.Max(p => p.Accuracy);

        public double MinAccuracy => Positions.Min(p => p.Accuracy);

        public IEnumerator<PositionEntry> GetEnumerator()
        {
            return Positions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
