using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Primitives;

namespace Tabi.Files.ResourceFilesSupport
{
    public class ResourceChangeToken : IChangeToken
    {
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => Disposable.Empty;
        public bool HasChanged => false;
        public bool ActiveChangeCallbacks => false;
    }
}
