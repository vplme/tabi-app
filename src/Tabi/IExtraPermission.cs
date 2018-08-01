namespace Tabi
{
    public interface IExtraPermission
    {
        bool RequestMotionPermission();

        PermissionAuthorization CheckMotionPermission();

        bool IsMotionAvailable();
    }
}
