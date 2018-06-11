using System;
using CsvHelper.Configuration;
using Tabi.Logic;

namespace Tabi.Logic
{
    public class ResolvedStopMap : ClassMap<ResolvedStop> 
    {
        public ResolvedStopMap()
        {
            Map(mn => mn.Latitude);
            Map(mn => mn.Longitude);
            Map(mn => mn.BeginTimestamp);
            Map(mn => mn.EndTimestamp);
            Map(mn => mn.TimeSpent);



        }
    }
}
