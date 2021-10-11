using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using XTricks.Geofencing.CrossInterfaces;
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

        public event EventHandler<LocationChangedEventArgs> LocationChanged;
        public event EventHandler<LocationDetectedEventArgs> LocationDetected;

        public bool IsRunning { get; private set; }
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

        public void Start()
        {
            LocationProvider.Start();

            IsRunning = true;
        }

        public void Stop()
        {
            LocationProvider.Stop();

            IsRunning = false;
        }

        public async Task PauseAsync(TimeSpan delay, CancellationToken token = default)
        {
            try
            {
                this.Stop();

                await Task.Delay(delay, token);

                this.Start();
            }
            catch (TaskCanceledException)
            {
                this.Start();
            }
        }

        internal GeofencingService(ILocationLogsStorage storage)
        {
            _monitoredLocations = new List<MonitoredLocation>();
            _locationLogsStorage = storage;
        }

        public async Task LocationChangedAsync(LocationLog log)
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

                locationsToRemove.Add(location);
            }

            if (!locationsToRemove.Any())
                return;

            foreach (var location in locationsToRemove)
                this.RemoveLocation(location.Key);
        }

        public MonitoredLocation GetLocation(object key)
        {
            return _monitoredLocations.FirstOrDefault(x => x.Key == key);
        }

        public void AddLocation(MonitoredLocation location)
        {
            if (location == null)
                throw new ArgumentException("Provided location is null");

            if (this.GetLocation(location.Key) != null)
            {
                this.RemoveLocation(location.Key);
            }

            this._monitoredLocations.Add(location);
        }

        public void RemoveLocation(object key)
        {
            var existing = this.GetLocation(key);

            if (existing == null)
                return;

            this._monitoredLocations.Remove(existing);
        }
    }
}
