using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Tabi.Files.LocalFilesSupport
{
    public class LocalFileInfo : IFileInfo
    {
        public LocalFileInfo(string path)
        {
            PhysicalPath = path;
            Name = Path.GetFileName(path);
        }

        public Stream CreateReadStream()
        {
            return new FileStream(PhysicalPath, FileMode.Open, FileAccess.Read);
        }

        public bool Exists
        {
            get
            {
                return File.Exists(PhysicalPath);
            }
        }

        public long Length => CreateReadStream().Length;
        public string PhysicalPath { get; }
        public string Name { get; }
        public DateTimeOffset LastModified => DateTimeOffset.Now;
        public bool IsDirectory => false;
    }
}
