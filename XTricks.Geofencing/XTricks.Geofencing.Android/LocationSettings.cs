using Android.App;
using System;

namespace XTricks.Geofencing.Droid
{
    public class LocationSettings
    {
        /// <summary>
        /// Interval used to collect location by the device
        /// </summary>
        public TimeSpan Interval { get; internal set; }
        /// <summary>
        /// The priority taken from <see cref="Android.Gms.Location.LocationRequest"/>
        /// </summary>
        public int Priority { get; internal set; }
        /// <summary>
        /// User interface notification
        /// </summary>
        public Notification Notification { get; internal set; }

        internal LocationSettings() { }
    }
}