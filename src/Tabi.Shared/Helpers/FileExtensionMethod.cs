using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PCLStorage;

namespace Tabi.Helpers
{
    public static class FileExtensionMethod
    {
        /// <summary>
        /// Opens a file, appends the specified string to the file, and then closes the file.
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="contents">The content to write to the file</param>
        /// <returns>A task which completes when the write operation finishes</returns>
        public static async Task AppendAllTextAsync(this IFile file, string contents)
        {
            using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
            {
                stream.Seek(stream.Length, SeekOrigin.Begin);
                using (var sw = new StreamWriter(stream))
                {
                    await sw.WriteAsync(contents).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Appends lines to a file, and then closes the file.
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="contents">The content to write to the file</param>
        /// <returns>A task which completes when the write operation finishes</returns>
        public static async Task AppendAllLinesAsync(this IFile file, IEnumerable<string> contents)
        {
            using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
            {
                stream.Seek(stream.Length, SeekOrigin.Begin);
                using (var sw = new StreamWriter(stream))
                {
                    foreach (var content in contents)
                    {
                        await sw.WriteLineAsync(content).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Appends lines to a file, and then closes the file.
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="contents">The content to write to the file</param>
        /// <returns>A task which completes when the write operation finishes</returns>
        public static async Task AppendAllLinesAsync(this IFile file, params string[] contents)
        {
            using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
            {
                stream.Seek(stream.Length, SeekOrigin.Begin);
                using (var sw = new StreamWriter(stream))
                {
                    foreach (var content in contents)
                    {
                        await sw.WriteLineAsync(content).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
