using System;

namespace XTricks.Geofencing.Droid
{
    public class GeofencingInitializer
    {
        /// <summary>
        /// Initializes settings for running location gathering background service
        /// </summary>
        /// <param name="func">Provides <see cref="LocationSettingsBuilder"/> to create <see cref="LocationSettings"/> object</param>
        public static void Initialize(Func<LocationSettingsBuilder, LocationSettings> func)
        {
            var settings = func(new LocationSettingsBuilder());

            Settings.LocationSettings = settings;
        }
    }
}