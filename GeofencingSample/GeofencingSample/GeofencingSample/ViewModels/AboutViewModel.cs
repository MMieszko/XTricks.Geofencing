using System;
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

        public AboutViewModel()
        {
            Title = "Stopped";
            StartGeofencingCommand = new Command(StartGeofencing);
            StopGoefencingCommand = new Command(StopGeofencing);
        }

        private void StopGeofencing(object obj)
        {
            if (!GeofencingService.Instance.IsRunning)
            {
                GeofencingService.Instance.Stop();
                this.Title = "Stopped";
            }
        }

        private void StartGeofencing(object obj)
        {
            var locationToMonitor = new MonitoredLocation(1, new LocationLog(51.759118, 19.45568, 0), Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Enter | GeofenceDirection.Exit);

            GeofencingService.Instance.AddLocation(locationToMonitor);

            GeofencingService.Instance.LocationDetected += GeofenceFired;

            GeofencingService.Instance.Start();

            this.Title = "Started";
        }

        private void GeofenceFired(object sender, LocationDetectedEventArgs e)
        {
            if (e.Direction == GeofenceDirection.Enter)
            {
                Title = "Entered";
            }
            else
            {
                Title = "Exited";
            }
        }

        public ICommand StartGeofencingCommand { get; }
        public ICommand StopGoefencingCommand { get; }
    }
}