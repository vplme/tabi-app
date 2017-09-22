using System;
using System.Globalization;
using CsvHelper.Configuration;
using Tabi.DataObjects;

namespace Tabi.Shared.Csv
{
    public class BatteryEntryMap : CsvClassMap<BatteryEntry>
    {
        public BatteryEntryMap()
        {
            CultureInfo customCulture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            Map(m => m.Timestamp).Index(0).TypeConverterOption(DateTimeStyles.AdjustToUniversal);
            Map(m => m.BatteryLevel).Index(1);
            Map(m => m.State).Index(2);
        }
  
    }
}
