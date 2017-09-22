using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tabi.DataObjects;

namespace Tabi.Shared
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
