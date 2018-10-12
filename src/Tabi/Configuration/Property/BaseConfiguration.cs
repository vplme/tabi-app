using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Tabi
{
    public class BaseConfiguration : ITabiPropertyConfiguration
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            dictionary.TryGetValue(propertyName, out object obj);

            T returnObject = default(T);
            if (obj != null)
            {
                returnObject = (T)obj;
            }

            return returnObject;
        }

        public bool HasProperty<T>([CallerMemberName] string propertyName = null)
        {
            return dictionary.ContainsKey(propertyName);
        }

        public void SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {


            if (dictionary.ContainsKey(propertyName))
            {
                dictionary.Remove(propertyName);
            }
            dictionary.Add(propertyName, value);
        }
    }
}
