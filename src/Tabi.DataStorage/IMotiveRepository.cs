using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface IMotiveRepository : IRepository<Motive>
    {
        /// <summary>
        /// Returns the last Motive matched with the given stop identifier.
        /// </summary>
        /// <returns>Last motive or null if it could not be found.</returns>
        /// <param name="stopId">Stop identifier.</param>
        Motive GetByStopVisitId(int stopVisitId);
        Motive GetByTrackId(int trackId);
        IEnumerable<Motive> StopMotivesAfter(DateTimeOffset begin);
        IEnumerable<Motive> TrackMotivesAfter(DateTimeOffset begin);

    }
}
