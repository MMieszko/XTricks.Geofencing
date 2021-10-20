using Android.Gms.Location;
using System;
using XTricks.Geofencing.Abstractions;

namespace XTricks.Geofencing.Android
{
    internal class GpsLocationCallback : LocationCallback
    {
        public override async void OnLocationResult(LocationResult result)
        {
            base.OnLocationResult(result);

            var log = new LocationLog(result.LastLocation.Latitude, result.LastLocation.Longitude, result.LastLocation.Accuracy, DateTime.Now);

            await GeofencingService.Instance.LocationChangedAsync(log);
        }
    }
}