using System;
namespace Tabi.Shared
{
    public interface IExtraPermission
    {
        bool RequestMotionPermission();

        PermissionAuthorization CheckMotionPermission();

        bool IsMotionAvailable();
    }
}
