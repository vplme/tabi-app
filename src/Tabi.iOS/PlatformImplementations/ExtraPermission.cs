using System;
using CoreMotion;
using Foundation;
using Tabi.Shared;
using UIKit;

namespace Tabi.iOS.PlatformImplementations
{
    public class ExtraPermission : IExtraPermission
    {
        private readonly CMMotionActivityManager _activityManager;

        public ExtraPermission(CMMotionActivityManager activityManager)

        {
            _activityManager = activityManager ?? throw new ArgumentNullException(nameof(activityManager));
        }


        private PermissionAuthorization ConvertToLocal(CMAuthorizationStatus authorizationStatus)
        {
            PermissionAuthorization permission = PermissionAuthorization.CheckNotAvailable;

            switch (authorizationStatus)
            {
                case CMAuthorizationStatus.Authorized:
                    permission = PermissionAuthorization.Authorized;
                    break;
                case CMAuthorizationStatus.Denied:
                    permission = PermissionAuthorization.Denied;
                    break;
                case CMAuthorizationStatus.NotDetermined:
                    permission = PermissionAuthorization.NotDetermined;
                    break;
                case CMAuthorizationStatus.Restricted:
                    permission = PermissionAuthorization.Restricted;
                    break;
            }

            return permission;
        }

        public PermissionAuthorization CheckMotionPermission()
        {
            PermissionAuthorization permission = PermissionAuthorization.CheckNotAvailable;

            // AuthorizationStatus onyl available > iOS 11. Check
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                CMAuthorizationStatus status = CMMotionActivityManager.AuthorizationStatus;
                permission = ConvertToLocal(status);

            }
            else
            {
                permission = PermissionAuthorization.CheckNotAvailable;
            }


            return permission;

        }

        public bool IsMotionAvailable()
        {
            return CMMotionActivityManager.IsActivityAvailable;
        }

        public bool RequestMotionPermission()
        {
            PermissionAuthorization status = CheckMotionPermission();

            if (status == PermissionAuthorization.NotDetermined){
                // There is no RequestPermission for CMMotionActivityManager. So we'll just start and stop recording.

                _activityManager.QueryActivity(NSDate.Now, NSDate.Now, NSOperationQueue.MainQueue, (activities, error) => { });
            }
            else if (status == PermissionAuthorization.Denied || status == PermissionAuthorization.Restricted) {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("app-settings:"));
            }

            return true;
        }
    }
}
