using System.Collections.Generic;
using Tabi.Model;
using SharpKml.Dom;
using SharpKml.Base;
using System.IO;
using CsvHelper;
using Tabi.Csv;
using Tabi.DataObjects;

namespace Tabi.Core
{
    public static class GeoUtil
    {
        public static string GeoSerialize(List<Position> positions)
        {
            Kml kml = new Kml();

            Placemark pMark = new Placemark();
            pMark.Name = "Tabi export";

            CoordinateCollection cl = new CoordinateCollection();
            SharpKml.Dom.GX.Track tr = new SharpKml.Dom.GX.Track();
            foreach (Position pos in positions)
            {
                Vector v = new Vector();
                v.Latitude = pos.Latitude;
                v.Longitude = pos.Longitude;
                cl.Add(v);

                tr.AddWhen(pos.Timestamp.DateTime);
                tr.AddCoordinate(v);
            }

            pMark.Geometry = tr;

            kml.Feature = pMark;

            Serializer serializer = new Serializer();
            serializer.Serialize(kml);

            return serializer.Xml;
        }

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