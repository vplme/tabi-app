using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.Logic
{
    public interface IStopResolver
    {
        IList<ResolvedStop> ResolveStops(IList<PositionEntry> positions); 
    }
}