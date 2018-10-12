using System;
using System.Runtime.CompilerServices;

namespace Tabi
{
    public class SwitchableConfiguration : ITabiPropertyConfiguration
    {
        private ITabiPropertyConfiguration _currentBacking;
        
        public SwitchableConfiguration(ITabiPropertyConfiguration newConfig)
        {
            _currentBacking = newConfig;
        }

        public void SetConfiguration(ITabiPropertyConfiguration newConfig)
        {
            _currentBacking = newConfig;
        }

        public T GetProperty<T>([CallerMemberName] string propertyName = null)
        {
            return _currentBacking.GetProperty<T>(propertyName);
        }

        public bool HasProperty<T>([CallerMemberName] string propertyName = null)
        {
            return _currentBacking.HasProperty<T>(propertyName);
        }

        public void SetProperty<T>(T value, [CallerMemberName] string propertyName = null)
        {
            _currentBacking.SetProperty<T>(value, propertyName);
        }
    }
}
