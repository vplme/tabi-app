using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Tabi.Csv;
using Tabi.DataObjects;

namespace Tabi.Core
{
    public static class GeoUtil
    {
        public static void PositionsToCsv(List<PositionEntry> positions, Stream stream)
        {
            using (TextWriter tw = new StreamWriter(stream))
            {
                var csv = new CsvWriter(tw);
                csv.Configuration.RegisterClassMap<PositionEntryMap>();
                csv.WriteRecords(positions);
            }
        }

        public static IEnumerable<PositionEntry> CsvToPositions(Stream stream)
        {
            IEnumerable<PositionEntry> entries;
            TextReader tr = new StreamReader(stream);

            var csv = new CsvReader(tr);
            csv.Configuration.RegisterClassMap<PositionEntryMap>();
            entries = csv.GetRecords<PositionEntry>();

            return entries;
        }
    }
}