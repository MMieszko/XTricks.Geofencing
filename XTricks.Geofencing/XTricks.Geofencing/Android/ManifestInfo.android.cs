using Android.App;

[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.ForegroundService)]
[assembly: UsesPermission(Android.Manifest.Permission.WakeLock)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessBackgroundLocation)]
//Required when targeting Android API 21+
[assembly: UsesFeature("android.hardware.location.gps")]
[assembly: UsesFeature("android.hardware.location.network")]