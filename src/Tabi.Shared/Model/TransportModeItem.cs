using System;
using System.Collections.Generic;
using Tabi.DataObjects;
using Tabi.Shared.Resx;
using static Tabi.ViewModels.TrackDetailViewModel;

namespace Tabi.Shared.Model
{
    public class TransportModeItem
    {
        public string Name { get; set; }
        public TransportationMode Mode { get; set; }

        public TransportModeItem()
        {

        }



        public static IList<TransportModeItem> GetPossibleTransportModes()
        {
            List<TransportModeItem> items = new List<TransportModeItem>();

            foreach (TransportationMode mode in Enum.GetValues(typeof(TransportationMode)))
            {
                TransportModeItem newItem = new TransportModeItem() { Name = GetTranslationForEnum(mode), Mode = mode };
                items.Add(newItem);
            }

            return items;
        }


        /// <summary>
        /// Gets the translation for a transportationmode enum.
        /// </summary>
        /// <returns>The translation for enum.</returns>
        /// <param name="mode">Transportmode enum</param>
        private static string GetTranslationForEnum(TransportationMode mode)
        {
            string result = "";

            switch (mode)
            {
                case TransportationMode.Bike:
                    result = AppResources.BikeLabel;
                    break;
                case TransportationMode.Bus:
                    result = AppResources.BusLabel;
                    break;
                case TransportationMode.Car:
                    result = AppResources.CarLabel;
                    break;
                case TransportationMode.MobilityScooter:
                    result = AppResources.MobilityScooterLabel;
                    break;
                case TransportationMode.Run:
                    result = AppResources.RunLabel;
                    break;
                case TransportationMode.Tram:
                    result = AppResources.TramLabel;
                    break;
                case TransportationMode.Walk:
                    result = AppResources.WalkLabel;
                    break;
                case TransportationMode.Moped:
                    result = AppResources.MopedLabel;
                    break;
                case TransportationMode.Motorcycle:
                    result = AppResources.MotorcycleLabel;
                    break;
                case TransportationMode.Other:
                    result = AppResources.OtherLabel;
                    break;
                case TransportationMode.Scooter:
                    result = AppResources.ScooterLabel;
                    break;
                case TransportationMode.Subway:
                    result = AppResources.SubwayLabel;
                    break;
                case TransportationMode.Train:
                    result = AppResources.TrainLabel;
                    break;
                default:
                    throw new ArgumentException($"{nameof(mode)} is not setup for translation.");
            }


            return result;
        }
        /// <summary>
        /// Retrieves the actual enums from a list of TransportModeItems. Which is a wrapper for the enums
        /// for the UI.
        /// </summary>
        /// <returns>List of transportmode enums that are in the list given to the method</returns>
        /// <param name="items">List of TransportModeItems (wrapped TransportationModeItems)</param>
        public static IList<TransportationMode> GetTransportModeEnums(IEnumerable<TransportModeItem> items)
        {
            List<TransportationMode> resultEnums = new List<TransportationMode>();
            foreach (TransportModeItem item in items)
            {
                resultEnums.Add(item.Mode);
            }

            return resultEnums;
        }
    }
}
