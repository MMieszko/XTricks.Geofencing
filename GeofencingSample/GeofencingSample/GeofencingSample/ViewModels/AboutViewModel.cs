using Plugin.Permissions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XTricks.Geofencing;
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

        public ICommand StartGeofencingCommand { get; }
        public ICommand PauseGeofencingCommand { get; }
        public ICommand StopGoefencingCommand { get; }

        public GeofenceDirection GeofenceStatus
        {
            get
            {
                return _geofenceStatus;
            }
            set
            {
                _geofenceStatus = value;
                OnPropertyChanged("GeofenceStatus");
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        public ILocation LastLocation
        {
            get
            {
                return _lastLocation;
            }
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
            StopGoefencingCommand = new Command(StopGeofencing);
            PauseGeofencingCommand = new Command(async () => await PauseGeofencingAsync());

            GeofencingService.Instance.LocationChanged += LocationChanged;
            GeofencingService.Instance.LocationDetected += GeofenceFired;
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
            var locationToMonitor = new MonitoredLocation(1, new LocationLog(51.759118, 19.45568, 0), Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Enter | GeofenceDirection.Exit);

            GeofencingService.Instance.AddLocation(locationToMonitor);

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
            this.GeofenceStatus = e.Direction;
        }
    }
}