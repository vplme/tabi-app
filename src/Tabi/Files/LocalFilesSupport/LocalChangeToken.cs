using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Primitives;

namespace Tabi.Files.LocalFilesSupport
{
    public class LocalChangeToken : IChangeToken
    {
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => Disposable.Empty;
        public bool HasChanged => false;
        public bool ActiveChangeCallbacks => false;
    }
}
