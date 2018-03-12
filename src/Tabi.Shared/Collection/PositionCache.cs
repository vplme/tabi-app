using System;
using Tabi.DataObjects;

namespace Tabi.Shared.Collection
{
    public class PositionCache
    {
        private PositionEntry initialPositionEntry;
        private PositionEntry previousPositionEntry;
        private PositionEntry prevpresPos;


        public double Distance { get; set; } = 100;

        public double PreviousDistance { get; private set; } = -1;
        
        public PositionCache()
        {
        }
        
        public bool Check(TimeSpan period)
        {
            return initialPositionEntry != null && DateTimeOffset.Now - initialPositionEntry.Timestamp >= period;
        }

        public void Add(PositionEntry entry)
        {
            prevpresPos = previousPositionEntry;
          
            if (initialPositionEntry == null)
            {
                initialPositionEntry = previousPositionEntry;
            }
            
            // Calculate distance between current and previous position
            if (previousPositionEntry != null)
            {
                PreviousDistance = previousPositionEntry.DistanceTo(entry);
            }

            // Use the accuracy of the current position to determine if
            // the current position could still be in range of the previous position.
            // Unless accuracy is (actual value) lower than the distance specified.
            double allowedDistance = entry.Accuracy < Distance ? Distance : entry.Accuracy;

            // Reset initial position if the new position compared to the previous and initial position
            // are outside the parameters
            if (initialPositionEntry != null && (initialPositionEntry.DistanceTo(entry) > allowedDistance || PreviousDistance > allowedDistance))
            {
                initialPositionEntry = null;
            }
            
            previousPositionEntry = entry;
        }
    }
}