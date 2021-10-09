﻿using Android.Gms.Location;
using System;

namespace XTricks.Geofencing.Droid
{
    internal class GpsLocationCallback : LocationCallback
    {
        public override void OnLocationResult(LocationResult result)
        {
            base.OnLocationResult(result);

            var log = new LocationLog(result.LastLocation.Latitude, result.LastLocation.Longitude, result.LastLocation.Accuracy, DateTime.Now);

            GeofencingService.Instance.LocationChangedAsync(log);
        }

        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {
            base.OnLocationAvailability(locationAvailability);
        }
    }
}