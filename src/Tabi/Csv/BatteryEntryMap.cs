using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Tabi.DataObjects;

namespace Tabi.Csv
{
    public class BatteryEntryMap : ClassMap<BatteryEntry>
    {
        public BatteryEntryMap()
        {
            CultureInfo customCulture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            Map(m => m.Timestamp).Index(0).TypeConverter<DateTimeOffsetConverter>();
            Map(m => m.BatteryLevel).Index(1);
            Map(m => m.State).Index(2);
        }

    }
}
