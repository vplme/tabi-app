using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Tabi
{
    public class LayeredConfiguration : ITabiPropertyConfiguration
    {
        SortedList<int, ITabiPropertyConfiguration> sortedConfigurations = new SortedList<int, ITabiPropertyConfiguration>();

        public LayeredConfiguration()
        {

        }

        public void AddConfiguration(int priority, ITabiPropertyConfiguration baseConfiguration)
        {
            if (sortedConfigurations.ContainsKey(priority))
            {
                sortedConfigurations.Remove(priority);
            }
            sortedConfigurations.Add(priority, baseConfiguration);
        }

        public T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            T result = default(T);
            bool found = false;
            foreach (ITabiPropertyConfiguration bc in sortedConfigurations.Values)
            {
                if (bc.HasProperty<T>(propertyName))
                {
                    result = bc.GetProperty<T>(propertyName);
                    found = true;
                    break;
                }
            }

            bool isSubConfig = result.GetType().IsSubclassOf(typeof(SubBaseConfiguration));
            Console.WriteLine($"Get {propertyName} {isSubConfig}");

            if (found && isSubConfig)
            {
                SubBaseConfiguration sub = result as SubBaseConfiguration;
                sub.OverrideBaseConfiguration(this);
            }

            return result;
        }

        public bool HasProperty<T>([CallerMemberName] string propertyName = null)
        {
            foreach (ITabiPropertyConfiguration bc in sortedConfigurations.Values)
            {
                if (bc.HasProperty<T>(propertyName))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {

        }
    }
}
