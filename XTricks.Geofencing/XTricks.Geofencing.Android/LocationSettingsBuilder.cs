using Android.App;
using System;

namespace XTricks.Geofencing.Droid
{
    public class LocationSettingsBuilder
    {
        private LocationSettings _settings;

        internal LocationSettingsBuilder()
        {
            _settings = new LocationSettings();
        }

        /// <summary>
        /// Set the interval value between gathering location logs from the device.
        /// The default value is 30 seconds
        /// </summary>
        /// <param name="value">Interval</param>
        public LocationSettingsBuilder WithInterval(TimeSpan value)
        {
            _settings.Interval = value;

            return this;
        }

        /// <summary>
        /// Set the priority of location request. The priority is connected with battery consumption.
        /// See <see cref="Android.Gms.Location.LocationRequest"/> and available options.
        /// The default value is <see cref="Android.Gms.Location.LocationRequest.PriorityBalancedPowerAccuracy"/>
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public LocationSettingsBuilder WithPriority(int priority)
        {
            _settings.Priority = priority;

            return this;
        }

        /// <summary>
        /// Notification which will be shown for the user while geofencing is turned on
        /// </summary>
        /// <param notification=""></param>
        /// <returns></returns>
        public LocationSettingsBuilder WithNotification(Notification notification)
        {
            _settings.Notification = notification;

            return this;
        }

        /// <summary>
        /// Creates <see cref="LocationSettings"/> object
        /// </summary>
        /// <returns></returns>
        public LocationSettings Build()
        {
            if (_settings.Notification == null)
                throw new Exception("Notification has to be set");

            return _settings;
        }
    }
}