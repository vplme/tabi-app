using System.Threading;
using System.Threading.Tasks;
using PCLStorage;

namespace Tabi.Helpers
{
    public class LogFileAccess
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1,1);
        private const string path = "tabi.log";

        public LogFileAccess()
        {

        }

        public async System.Threading.Tasks.Task AppendAsync(string s)
        {
            await semaphore.WaitAsync();
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                var file = await rootFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists);
                await file.AppendAllTextAsync(s);
            }
            finally
            {
                semaphore.Release();

            }


        }

        public async Task<string> ReadFile()
        {
            await semaphore.WaitAsync();

            string text = "no log";
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                IFile file = await rootFolder.GetFileAsync(path);
                text = await file.ReadAllTextAsync();
            }
            finally
            {
                semaphore.Release();



            }

            return text;
        }
    }
}

