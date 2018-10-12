using System;
using System.Runtime.CompilerServices;

namespace Tabi
{
    public class SubBaseConfiguration : ITabiPropertyConfiguration
    {
        ITabiPropertyConfiguration _baseConfiguration;
        bool replaced = false;
        readonly string _className;

        public SubBaseConfiguration(ITabiPropertyConfiguration baseConfiguration)
        {
            _baseConfiguration = baseConfiguration;
            _className = this.GetType().Name;
        }

        public T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            return _baseConfiguration.GetProperty<T>($"{_className}.{propertyName}");
        }

        public bool HasProperty<T>([CallerMemberName] string propertyName = null)
        {
            return _baseConfiguration.HasProperty<T>($"{_className}.{propertyName}");
        }

        public void SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            _baseConfiguration.SetProperty<T>(value, $"{_className}.{propertyName}");
        }

        public void OverrideBaseConfiguration(ITabiPropertyConfiguration configuration)
        {
            if (!replaced){
                replaced = true;
                _baseConfiguration = configuration;
            }
        }
    }
}
