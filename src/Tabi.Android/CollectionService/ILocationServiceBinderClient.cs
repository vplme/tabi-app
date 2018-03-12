using System;
namespace Tabi.Droid
{
    public interface ILocationServiceBinderClient
    {
        LocationServiceBinder Binder { get; set; }
        bool IsBound { get; set; }

        void OnBinding();
    }
}
