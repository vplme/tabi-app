using System.Collections.Generic;
using Tabi.Resx;

namespace Tabi.Model
{
    public class TransportModeItem
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public TransportModeItem()
        {

        }

        public static List<TransportModeItem> GetPossibleTransportModes(TransportationModeConfiguration configuration)
        {
            List<TransportModeItem> items = new List<TransportModeItem>();

            if (configuration.Options != null)
            {
                foreach (TransportOption mode in configuration.Options)
                {
                    string translatedText = typeof(AppResources).GetProperty($"{mode.Id}TransportText")?.GetValue(null) as string;
                    string text = translatedText ?? mode.Text;

                    TransportModeItem newItem = new TransportModeItem() { Name = text, Id = mode.Id };
                    items.Add(newItem);
                }
            }

            return items;
        }

        /// <summary>
        /// Retrieves the actual enums from a list of TransportModeItems. Which is a wrapper for the enums
        /// for the UI.
        /// </summary>
        /// <returns>List of transportmode enums that are in the list given to the method</returns>
        /// <param name="items">List of TransportModeItems (wrapped TransportationModeItems)</param>
        public static IList<string> GetTransportModeEnums(IEnumerable<TransportModeItem> items)
        {
            List<string> resultEnums = new List<string>();
            foreach (TransportModeItem item in items)
            {
                resultEnums.Add(item.Id);
            }

            return resultEnums;
        }
    }
}
