using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using XTricks.Geofencing.Abstractions;
using XTricks.Shared.Contracts;

namespace XTricks.Geofencing
{
    public sealed class GeofencingService
    {
        private static readonly SemaphoreSlim Semaphore;
        private static GeofencingService _instance;
        private static ILocationProvider LocationProvider => DependencyService.Get<ILocationProvider>();
        private static IPermissionsManager PermissionsManager => DependencyService.Get<IPermissionsManager>();

        private readonly ILocationLogsStorage _locationLogsStorage;
        private readonly List<MonitoredLocation> _monitoredLocations;

        /// <summary>
        /// Fires whenever new location from device appeared to use by geofencing
        /// </summary>
        public event EventHandler<LocationChangedEventArgs> LocationChanged;

        /// <summary>
        /// Fires whenever any <see cref="MonitoredLocation"/> is being catch as <see cref="GeofenceDirection.Exit"/> or <see cref="GeofenceDirection.Enter"/>
        /// When monitored location is detected it automatically gets removed from search
        /// </summary>
        public event EventHandler<LocationDetectedEventArgs> LocationDetected;

        /// <summary>
        /// Provides information if GeofencingService is being paused.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Provides information if GeofencingService is running. 
        /// Notice that IsRunning is set to false when <see cref="PauseAsync(TimeSpan, CancellationToken)"/> is requested.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Provides the readonly collection of current monitored locations
        /// </summary>
        public IReadOnlyCollection<MonitoredLocation> MonitoredLocations => _monitoredLocations.AsReadOnly();

        private GeofencingService()
        {

        }

        static GeofencingService()
        {
            Semaphore = new SemaphoreSlim(1);
        }

        public static GeofencingService Instance
        {
            get { return _instance ??= new GeofencingService(new InMemoryLocationsStorage(20)); }
        }

        internal GeofencingService(ILocationLogsStorage storage)
        {
            _monitoredLocations = new List<MonitoredLocation>();
            _locationLogsStorage = storage;
        }

        /// <summary>
        /// Starts the service
        /// </summary>
        /// <returns>Returns <see cref="StartResult"/> which brings the information if start became succeeded else provides information about the failure</returns>
        public StartResult Start()
        {
            try
            {
                var permissionCheckResult = PermissionsManager.Check();

                if (!permissionCheckResult)
                    return StartResult.CreateFailed(StartFailureType.MissingPermissions, null, permissionCheckResult.MissingPermission);

                var result = LocationProvider.Start();

                if (!result.Succeeded)
                    return result;

                IsRunning = true;

                return StartResult.CreateSucceeded();
            }
            catch (Exception ex)
            {
                return StartResult.CreateFailed(StartFailureType.Other, ex);
            }
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        public void Stop()
        {
            LocationProvider.Stop();

            IsRunning = false;
            IsPaused = false;
        }

        /// <summary>
        /// Pauses the services. Pause is basically turning off and turning on after given period of time
        /// </summary>
        /// <param name="delay">Pause duration</param>
        /// <param name="token">Token and its cancellation which is able to break the delay and bring back service running immediately</param>
        /// <returns></returns>
        public async Task PauseAsync(TimeSpan delay, CancellationToken token = default)
        {
            try
            {
                if (this.IsPaused || !this.IsRunning)
                    return;

                this.Stop();

                this.IsPaused = true;

                await Task.Delay(delay, token);

                this.Start();
            }
            catch (TaskCanceledException)
            {
                this.Start();
                this.IsPaused = false;
            }
        }

        /// <summary>
        /// Return <see cref="MonitoredLocation"/> whenever service contains it
        /// If such location does not exists the method will return null.
        /// </summary>
        /// <param name="key">Key identifier of <see cref="MonitoredLocation"/></param>
        /// <returns>Monitored location</returns>
        public MonitoredLocation GetLocation(object key)
        {
            return _monitoredLocations.FirstOrDefault(x => x.Key == key);
        }

        /// <summary>
        /// Adds <see cref="MonitoredLocation"/> into service
        /// If key of given location already exists then the location will be replaced.
        /// </summary>
        public void AddLocation(MonitoredLocation location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            if (this.GetLocation(location.Key) != null)
            {
                this.RemoveLocation(location.Key);
            }

            this._monitoredLocations.Add(location);
        }

        /// <summary>
        /// Adds collection of <see cref="MonitoredLocation"/> into service.
        /// Reuses the method <see cref="AddLocation(MonitoredLocation)"/>
        /// </summary>
        /// <param name="monitoredLocations"></param>
        public void AddLocations(IEnumerable<MonitoredLocation> monitoredLocations)
        {
            foreach (var location in monitoredLocations)
                this.AddLocation(location);
        }

        /// <summary>
        /// Removed monitored location by given key.
        /// If location is not found the method will do nothing
        /// </summary>
        public void RemoveLocation(object key)
        {
            var existing = this.GetLocation(key);

            if (existing == null)
                return;

            this._monitoredLocations.Remove(existing);
        }

        /// <summary>
        /// Removes monitored location
        /// If location is not found the method will do nothing
        /// </summary>
        public void RemoveLocation(MonitoredLocation location)
        {
            if (location == null)
                return;

            this.RemoveLocation(location.Key);
        }

        /// <summary>
        /// Removes given locations.
        /// Reuses <see cref="RemoveLocation(object)"/> method
        /// </summary>
        /// <param name="locations"></param>
        public void RemoveLocations(IEnumerable<MonitoredLocation> locations)
        {
            foreach (var location in locations)
                this.RemoveLocation(location);
        }

        /// <summary>
        /// Removes all monitoring locations
        /// </summary>
        public void RemoveLocations()
        {
            this._monitoredLocations.Clear();
        }

        public async Task LocationChangedAsync(LocationLog log)
        {
            try
            {
                await Semaphore.WaitAsync();

                this.LocationChanged?.Invoke(this, new LocationChangedEventArgs(log));

                await _locationLogsStorage.AddAsync(log);

                if (this.MonitoredLocations.Count == 0)
                    return;

                var logs = await _locationLogsStorage.GetAsync();

                var triggers = new Dictionary<MonitoredLocation, GeofenceDirection>();

                foreach (var location in MonitoredLocations)
                {
                    var result = location.Detector.Detect(logs.Cast<ILocation>().ToList());

                    if (result == GeofenceDirection.None)
                        continue;

                    triggers.Add(location, result);
                }

                this.RemoveLocations(triggers.Select(x => x.Key));

                foreach (var trigger in triggers)
                {
                    LocationDetected?.Invoke(this, new LocationDetectedEventArgs(trigger.Key, trigger.Value));
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
