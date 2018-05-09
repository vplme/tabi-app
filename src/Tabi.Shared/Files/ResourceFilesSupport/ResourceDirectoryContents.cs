using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace Tabi.Shared.Config
{
    public class ResourceDirectoryContents : IDirectoryContents
    {
        public IEnumerator<IFileInfo> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Exists { get; }
    }
}
