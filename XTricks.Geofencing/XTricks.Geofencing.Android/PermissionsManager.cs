using Android;
using Android.Content.PM;
using AndroidX.Core.Content;
using System.Threading.Tasks;
using Xamarin.Essentials;
using XTricks.Geofencing.Droid;
using static Xamarin.Essentials.Permissions;

[assembly: Xamarin.Forms.Dependency(typeof(PermissionsManager))]
namespace XTricks.Geofencing.Droid
{
    public class PermissionsManager : IPermissionsManager
    {
        public const int WakeLockPermissionCode = 5555;
        public const int FineLocationPermissions = 5556;

        private const string WakeLockPermission = Manifest.Permission.WakeLock;
        private const string LocationPermission = Manifest.Permission.AccessFineLocation;

        public PermissionCheckResult Check()
        {

            var wakeLockPermission = ContextCompat.CheckSelfPermission(Platform.CurrentActivity, WakeLockPermission);

            if (wakeLockPermission != Permission.Granted)
                return PermissionCheckResult.CreateFailed(WakeLockPermission);

            var locationPermission = ContextCompat.CheckSelfPermission(Platform.CurrentActivity, LocationPermission);

            if (locationPermission != Permission.Granted)
                return PermissionCheckResult.CreateFailed(LocationPermission);

            return PermissionCheckResult.CreateSucceeded();
        }
    }
}