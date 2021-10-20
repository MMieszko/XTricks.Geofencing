using System;
using Xamarin.Forms;
using XTricks.Geofencing.Abstractions;

namespace XTricks.Geofencing.Android
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

            DependencyService.Register<IPermissionsManager, PermissionsManager>();
            DependencyService.Register<ILocationProvider, LocationProvider>();

            Settings.LocationSettings = settings;
        }
    }
}