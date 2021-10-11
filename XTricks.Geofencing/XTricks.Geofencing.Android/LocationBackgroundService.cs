using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Xamarin.Essentials;
using XTricks.Geofencing.CrossInterfaces;
using XTricks.Geofencing.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(LocationBackgroundService))]
namespace XTricks.Geofencing.Droid
{
    [Service]
    public class LocationBackgroundService : Service, ILocationProvider
    {
        public const int WakeLockPermissionCode = 5555;
        public const int FineLocationPermissions = 5556;
        public const int ServiceId = 5555;

        private PowerManager.WakeLock _wakeLock;
        private LocationRequest _locationRequest;
        private FusedLocationProviderClient _locationProvider;
        private Intent _intent;
        private GpsLocationCallback _locationCallback;

        public override IBinder OnBind(Intent intent) => null;

        public override void OnCreate()
        {
            base.OnCreate();

            _locationRequest = LocationRequest.Create();
            _locationRequest.SetPriority(Settings.LocationSettings.Priority);
            _locationRequest.SetInterval((long)Settings.LocationSettings.Interval.TotalMilliseconds);
            _locationRequest.SetFastestInterval((long)Settings.LocationSettings.Interval.TotalMilliseconds);

            _locationProvider = new FusedLocationProviderClient(this);
            _locationCallback = new GpsLocationCallback();
            _locationProvider.RequestLocationUpdatesAsync(_locationRequest, _locationCallback);
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);

            var powerManager = (PowerManager)GetSystemService(PowerService);

            if (_wakeLock == null)
                _wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "GeofencingWakeLock");

            if (_wakeLock.IsHeld)
                return StartCommandResult.Sticky;

            var wakeLockPremissions = ContextCompat.CheckSelfPermission(this, Manifest.Permission.WakeLock);

            if (wakeLockPremissions != Permission.Granted)
                ActivityCompat.RequestPermissions(Platform.CurrentActivity, new[] { Manifest.Permission.WakeLock }, WakeLockPermissionCode);

            var locationPermissions = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

            if (locationPermissions != Permission.Granted)
                ActivityCompat.RequestPermissions(Platform.CurrentActivity, new[] { Manifest.Permission.AccessFineLocation }, FineLocationPermissions);

            _wakeLock.Acquire();

            StartForeground(ServiceId, Settings.LocationSettings.Notification);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            if (_locationProvider != null && _locationCallback != null)
            {
                _locationProvider.RemoveLocationUpdatesAsync(_locationCallback);
                _locationProvider = null;
                _locationRequest = null;
            }

            if (_wakeLock != null)
            {
                _wakeLock.Release();
                _wakeLock = null;
            }

            base.OnDestroy();
        }

        public void Start()
        {
            try
            {
                var activity = Platform.CurrentActivity;

                _intent = new Intent(activity, typeof(LocationBackgroundService));

                activity.StartService(_intent);
            }
            catch (Java.Lang.Exception ex)
            {

            }
            catch (System.Exception ex)
            {

            }
        }

        public void Stop()
        {
            try
            {
                var activity = Platform.CurrentActivity;

                if (activity != null && _intent != null)
                    activity.StopService(_intent);
            }
            catch (Java.Lang.Exception ex)
            {

            }
            catch (System.Exception ex)
            {

            }
        }
    }
}