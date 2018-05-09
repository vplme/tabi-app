using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Tabi.Shared.Config
{
    public class ResourceFileProvider : IFileProvider
{
    public IFileInfo GetFileInfo(string subpath)
    {
        return new ResourceFileInfo(subpath);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return new ResourceDirectoryContents();
    }

    public IChangeToken Watch(string filter)
    {
        return new ResourceChangeToken();
    }

}
}
