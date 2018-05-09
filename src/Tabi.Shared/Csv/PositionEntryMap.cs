using System.Globalization;
using CsvHelper.Configuration;
using Tabi.DataObjects;

namespace Tabi.Csv
{
    public class PositionEntryMap : ClassMap<PositionEntry>
    {
        public PositionEntryMap()
        {
            CultureInfo customCulture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            //Map(m => m.Timestamp).Index(0).TypeConverterOption(DateTimeStyles.AdjustToUniversal);
            //Map(m => m.Latitude).Index(1).TypeConverterOption(customCulture);
            //Map(m => m.Longitude).Index(2).TypeConverterOption(customCulture);
            //Map(m => m.Accuracy).Index(3);
            //Map(m => m.DesiredAccuracy).Index(4);
            //Map(m => m.DistanceBetweenPreviousPosition).Index(5);
        }
    }
}