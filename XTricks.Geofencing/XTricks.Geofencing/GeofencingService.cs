﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using XTricks.Geofencing.CrossInterfaces;
using XTricks.Geofencing.Droid;
using XTricks.Geofencing.Storage;
using XTricks.Shared.Contracts;

namespace XTricks.Geofencing
{
    public sealed class GeofencingService
    {
        private static GeofencingService _instance;
        private ILocationProvider LocationProvider => DependencyService.Get<ILocationProvider>();

        private readonly ILocationLogsStorage _locationLogsStorage;
        private readonly List<MonitoredLocation> _monitoredLocations;

        /// <summary>
        /// Fires whenever new location from device appeared to use by geofencing
        /// </summary>
        public event EventHandler<LocationChangedEventArgs> LocationChanged;

        /// <summary>
        /// Fires whenever any <see cref="MonitoredLocation"/> is being catched as <see cref="GeofenceDirection.Exit"/> or <see cref="GeofenceDirection.Enter"/>
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
        /// Provicdes the readonly collection of current monitored locations
        /// </summary>
        public IReadOnlyCollection<MonitoredLocation> MonitoredLocations => _monitoredLocations.AsReadOnly();

        private GeofencingService()
        {

        }

        public static GeofencingService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GeofencingService(new InMemoryLocationsStorage());

                return _instance;
            }
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
                var permissionsManager = DependencyService.Get<IPermissionsManager>();
                var permissionCheckResult = permissionsManager.Check();

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
        /// Pauses the services. Pause is basically turning off and turnign on after given period of time
        /// </summary>
        /// <param name="delay">Puase duration</param>
        /// <param name="token">Token and its cancellation which is able to break the delay and bring back service running immeadetly</param>
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
        /// <param name="key">Key identitifer of <see cref="MonitoredLocation"/></param>
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
        /// Removed monitored location
        /// If location is not found the method will do nothing
        /// </summary>
        public void RemoveLocation(MonitoredLocation location)
        {
            if (location == null)
                return;

            this.RemoveLocation(location.Key);
        }

        internal async Task LocationChangedAsync(LocationLog log)
        {
            this.LocationChanged?.Invoke(this, new LocationChangedEventArgs(log));

            await _locationLogsStorage.AddAsync(log);

            if (this.MonitoredLocations.Count == 0)
                return;

            var logs = await _locationLogsStorage.GetAsync();
            var locationsToRemove = new List<MonitoredLocation>();

            foreach (var location in MonitoredLocations)
            {
                var result = location.Detector.Detect(logs.Cast<ILocation>().ToList());

                if (result == GeofenceDirection.None)
                    continue;

                LocationDetected?.Invoke(this, new LocationDetectedEventArgs(location, result));

                if (location.RemoveAfterDetected)
                    locationsToRemove.Add(location);
            }

            if (!locationsToRemove.Any())
                return;

            foreach (var location in locationsToRemove)
                this.RemoveLocation(location.Key);
        }
    }
}
