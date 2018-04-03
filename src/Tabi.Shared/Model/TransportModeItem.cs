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
        public TransportationModes Mode { get; set; }

        public TransportModeItem()
        {

        }



        public static IList<TransportModeItem> GetPossibleTransportModes()
        {
            List<TransportModeItem> items = new List<TransportModeItem>();

            foreach (TransportationModes mode in Enum.GetValues(typeof(TransportationModes)))
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
        private static string GetTranslationForEnum(TransportationModes mode)
        {
            string result = "";

            switch (mode)
            {
                case TransportationModes.Bike:
                    result = AppResources.BikeLabel;
                    break;
                case TransportationModes.Bus:
                    result = AppResources.BusLabel;
                    break;
                case TransportationModes.Car:
                    result = AppResources.CarLabel;
                    break;
                case TransportationModes.MobilityScooter:
                    result = AppResources.MobilityScooterLabel;
                    break;
                case TransportationModes.Run:
                    result = AppResources.RunLabel;
                    break;
                case TransportationModes.Tram:
                    result = AppResources.TramLabel;
                    break;
                case TransportationModes.Walk:
                    result = AppResources.WalkLabel;
                    break;
                case TransportationModes.Moped:
                    result = AppResources.MopedLabel;
                    break;
                case TransportationModes.Motorcycle:
                    result = AppResources.MotorcycleLabel;
                    break;
                case TransportationModes.Other:
                    result = AppResources.OtherLabel;
                    break;
                case TransportationModes.Scooter:
                    result = AppResources.ScooterLabel;
                    break;
                case TransportationModes.Subway:
                    result = AppResources.SubwayLabel;
                    break;
                case TransportationModes.Train:
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
        public static IList<TransportationModes> GetTransportModeEnums(IEnumerable<TransportModeItem> items)
        {
            List<TransportationModes> resultEnums = new List<TransportationModes>();
            foreach (TransportModeItem item in items)
            {
                resultEnums.Add(item.Mode);
            }

            return resultEnums;
        }
    }
}
