using System.Runtime.CompilerServices;

namespace Tabi
{
    public interface ITabiPropertyConfiguration
    {
        T GetProperty<T>([CallerMemberName] string propertyName = null);
        bool HasProperty<T>([CallerMemberName] string propertyName = null);
        void SetProperty<T>(T value, [CallerMemberName] string propertyName = null);
    }
}