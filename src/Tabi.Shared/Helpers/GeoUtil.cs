using System;
using System.Collections.Generic;
using Tabi.Model;

using SharpKml.Dom;
using SharpKml.Base;
using SharpKml.Dom.GX;
using System.Text;
using System.Globalization;
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

            //MultipleGeometry multiGeometry = new MultipleGeometry();
            //multiGeometry.AddGeometry(new LineString() { Coordinates = cl, });
            //multiGeometry.AddGeometry(tr);

            pMark.Geometry = tr;

            kml.Feature = pMark;

            Serializer serializer = new Serializer();
            serializer.Serialize(kml);

            return serializer.Xml;
        }

        public static string PositionsToCsv(List<PositionEntry> positions)
        {
            StringBuilder csvBuilder = new StringBuilder();
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            csvBuilder.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"{6}", "Timestamp", "Latitude", "Longitude", "Accuracy", "DesiredAccuracy", "DistanceBetween", Environment.NewLine);
            foreach (PositionEntry p in positions)
            {
                csvBuilder.AppendFormat(nfi, "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"{6}", p.Timestamp.ToUniversalTime(), p.Latitude, p.Longitude, p.Accuracy, p.DesiredAccuracy, p.DistanceBetweenPreviousPosition, Environment.NewLine);
            }

            return csvBuilder.ToString();

        }
    }
}
