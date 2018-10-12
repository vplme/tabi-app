using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Tabi.Files.LocalFilesSupport
{
    public class LocalFileProvider : IFileProvider
    {
        public IFileInfo GetFileInfo(string subpath)
        {
            return new LocalFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new LocalDirectoryContents();
        }

        public IChangeToken Watch(string filter)
        {
            return new LocalChangeToken();
        }
    }
}
