using System;
using CsvHelper.Configuration;
using Tabi.DataObjects;

namespace Tabi.Logic
{
    public sealed class PositionEntryMap : ClassMap<PositionEntry>
    {
        public PositionEntryMap()
        {
            Map(m => m.Id).Ignore();
            Map(m => m.DeviceId).Name("device_id");
            Map(m => m.Latitude).Name("latitude");
            Map(m => m.Longitude).Name("longitude");
            Map(m => m.Accuracy).Name("accuracy");
            Map(m => m.Speed).Name("speed");
            Map(m => m.Altitude).Name("altitude");
            Map(m => m.DesiredAccuracy).Name("desired_accuracy");
            Map(m => m.Timestamp).Name("timestamp");
        }
    }
}
