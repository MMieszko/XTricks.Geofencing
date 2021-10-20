using Plugin.Permissions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XTricks.Geofencing;
using XTricks.Geofencing.Abstractions;
using XTricks.Shared.Contracts;
using XTricks.Shared.Models;

namespace GeofencingSample.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private MonitoredLocation _monitoredLocation;
        private ILocation _lastLocation;
        private bool _isRunning;
        private GeofenceDirection _geofenceStatus;
        private ObservableCollection<string> _logs;

        public ICommand StartGeofencingCommand { get; }
        public ICommand PauseGeofencingCommand { get; }
        public ICommand StopGeofencingCommand { get; }

        public ObservableCollection<string> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged("Logs");
            }
        }

        public GeofenceDirection GeofenceStatus
        {
            get => _geofenceStatus;
            set
            {
                _geofenceStatus = value;
                OnPropertyChanged("GeofenceStatus");
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        public ILocation LastLocation
        {
            get => _lastLocation;
            set
            {
                _lastLocation = value;
                OnPropertyChanged("LastLocation");
            }
        }

        public MonitoredLocation MonitoredLocation
        {
            get
            {
                return _monitoredLocation;
            }
            set
            {
                _monitoredLocation = value;

                OnPropertyChanged("MonitoredLocation");
            }
        }

        public AboutViewModel()
        {
            Title = "Geofencing service";

            GeofenceStatus = GeofenceDirection.None;
            StartGeofencingCommand = new Command(async () => await StartGeofencingAsync());
            StopGeofencingCommand = new Command(StopGeofencing);
            PauseGeofencingCommand = new Command(async () => await PauseGeofencingAsync());
            Logs = new ObservableCollection<string>();

            GeofencingService.Instance.LocationChanged += LocationChanged;
            GeofencingService.Instance.LocationDetected += GeofenceFired;

            var lidl = new MonitoredLocation("Lidl", 51.6174298, 15.3111643, Distance.FromMeters(100), Distance.FromMeters(200), GeofenceDirection.Enter | GeofenceDirection.Exit);
            var dom = new MonitoredLocation("Dom", 51.6127332, 15.322100, Distance.FromMeters(100), Distance.FromMeters(200), GeofenceDirection.Enter | GeofenceDirection.Exit);
            var polo = new MonitoredLocation("Polo", 51.6127328, 15.331042, Distance.FromMeters(100), Distance.FromMeters(200), GeofenceDirection.Enter | GeofenceDirection.Exit);
            var kaufland = new MonitoredLocation("Kaufland", 51.6163051, 15.313698, Distance.FromMeters(100), Distance.FromMeters(200), GeofenceDirection.Enter | GeofenceDirection.Exit);


            GeofencingService.Instance.AddLocations(new[] { lidl, dom, polo, kaufland });
        }

        private async Task PauseGeofencingAsync()
        {
            await GeofencingService.Instance.PauseAsync(TimeSpan.FromSeconds(10));
        }

        private void StopGeofencing(object obj)
        {
            if (GeofencingService.Instance.IsRunning)
            {
                GeofencingService.Instance.Stop();
                IsRunning = false;
            }
        }

        private async Task StartGeofencingAsync()
        {
            var startResult = GeofencingService.Instance.Start();

            if (startResult.Succeeded)
            {
                IsRunning = true;
            }
            else if (startResult.Type == StartFailureType.MissingPermissions)
            {
                var status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();

                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    await StartGeofencingAsync();
                }
            }
        }

        private void LocationChanged(object sender, LocationChangedEventArgs e)
        {
            LastLocation = e.Location;
        }

        private void GeofenceFired(object sender, LocationDetectedEventArgs e)
        {
            this.Logs.Add($"[{e.DateTime.Hour}:{e.DateTime.Minute}:{e.DateTime.Second}]  [{e.Location.Key}] ->  [{e.Direction}]");

            this.GeofenceStatus = e.Direction;

            if (e.Direction == GeofenceDirection.Enter)
            {
                GeofencingService.Instance.AddLocation(new MonitoredLocation(e.Location.Key, e.Location.Location, e.Location.RadiusEnter, e.Location.RadiusExit, GeofenceDirection.Exit));
            }
            else if (e.Direction == GeofenceDirection.Exit)
            {
                GeofencingService.Instance.AddLocation(new MonitoredLocation(e.Location.Key, e.Location.Location, e.Location.RadiusEnter, e.Location.RadiusExit, GeofenceDirection.Enter));
            }
        }
    }
}