using System;
using System.IO;
using System.Reflection;
using PCLStorage;
using Xamarin.Forms;

namespace Tabi
{
    public class DummyDbLoader
    {
        public DummyDbLoader()
        {
        }

        public async System.Threading.Tasks.Task LoadAsync()
        {
            var assembly = typeof(DummyDbLoader).GetTypeInfo().Assembly;
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var file = await rootFolder.CreateFileAsync("tabi", CreationCollisionOption.ReplaceExisting);

            using (Stream readStream = assembly.GetManifestResourceStream("Tabi.sample_tabi"))
            using (Stream writeStream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
            {
                await readStream.CopyToAsync(writeStream); 
            }

            Application.Current.Properties["latestProcessedDate"] = DateTimeOffset.MinValue.UtcTicks;

        }
    }
}
