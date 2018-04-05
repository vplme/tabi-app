using System.IO;
using PCLStorage;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Tabi.Droid;
using FileAccess = PCLStorage.FileAccess;
using Permission = Plugin.Permissions.Abstractions.Permission;

[assembly: Xamarin.Forms.Dependency(typeof(ShareFileImplementation))]
namespace Tabi.Droid
{
    public class ShareFileImplementation : IShareFile
    {
        private const string externalStorageFolder = "Tabi/";
        
        // On Android this implementation saves the file to external storage (world readable)
        // instead of sharing it with apps.
        public void ShareFile(string path, string mime = "text/plain")
        {
            Log.Info($"Sharing file with path: {path}");
        
            IFile toBeSharedFile = FileSystem.Current.GetFileFromPathAsync(path).Result;

            var externalPath = Android.OS.Environment.ExternalStorageDirectory.ToString();
            var filePath = System.IO.Path.Combine(externalPath, toBeSharedFile.Name);


            if (PermissionStatus.Granted != CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage).Result)
            {
                CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage).Wait();
            }
            Stream stream = toBeSharedFile.OpenAsync(FileAccess.ReadAndWrite).Result;
            
            using (StreamReader streamReader = new StreamReader(stream))
            {
                using (var streamWriter = new StreamWriter(filePath, false))
                {
                    streamWriter.Write(streamReader.ReadToEnd());
                }

            }
            
        }
    }
}
