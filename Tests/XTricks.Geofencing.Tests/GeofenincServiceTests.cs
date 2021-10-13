using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using XTricks.Geofencing.Storage;
using XTricks.Shared.Models;

namespace XTricks.Geofencing.Tests
{
    public class GeofenincServiceTests
    {
        private GeofencingService _geofencingService;

        [SetUp]
        public void Setup()
        {
            _geofencingService = new GeofencingService(new InMemoryLocationsStorage());
        }

        [Test]
        public void AddLocation()
        {
            var location = new MonitoredLocation(new object(), new LocationLog(1d, 1d, 1d), Distance.FromMeters(100), Distance.FromMeters(150), GeofenceDirection.Enter);

            _geofencingService.AddLocation(location);

            _geofencingService.MonitoredLocations.Should().HaveCount(1);
        }

        [Test]
        public void AddLocation_AddTheSame_ShouldReplace()
        {
            var key = new object();

            var location1 = new MonitoredLocation(key, new LocationLog(1d, 1d, 1d), Distance.FromMeters(100), Distance.FromMeters(150), GeofenceDirection.Enter);
            var location2 = new MonitoredLocation(key, new LocationLog(5d, 5d, 5d), Distance.FromMeters(100), Distance.FromMeters(150), GeofenceDirection.Enter);

            _geofencingService.AddLocation(location1);
            _geofencingService.AddLocation(location2);

            _geofencingService.MonitoredLocations.Should().HaveCount(1);
            _geofencingService.MonitoredLocations.First().Location.Latitude.Should().Be(5d);
        }

        [Test]
        public void RemoveLocation()
        {
            var key = new object();

            var location = new MonitoredLocation(key, new LocationLog(1d, 1d, 1d), Distance.FromMeters(100), Distance.FromMeters(150), GeofenceDirection.Enter);

            _geofencingService.AddLocation(location);
            _geofencingService.RemoveLocation(key);

            _geofencingService.MonitoredLocations.Should().HaveCount(0);
        }

        [Test]
        public void RemoveLocation_LocationNotExists_DoNothing()
        {
            var key = new object();

            _geofencingService.RemoveLocation(key);

            _geofencingService.MonitoredLocations.Should().HaveCount(0);
        }

        [Test]
        public void GetLocation()
        {
            var key = new object();

            var location = new MonitoredLocation(key, new LocationLog(1d, 1d, 1d), Distance.FromMeters(100), Distance.FromMeters(150), GeofenceDirection.Enter);

            _geofencingService.AddLocation(location);

            var found = _geofencingService.GetLocation(key);

            found.Should().Be(location);
        }

        [Test]
        public async Task LocationChangedAsync_OneMonitoring_OneLocation_NoAction()
        {
            var key = new object();
            var lat = 28.5799448;
            var lng = -16.1348016;
            var log = new LocationLog(lat, lng);
            bool assert = false;

            var monitoredLocation = new MonitoredLocation(key, lat, lng, Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Enter);

            _geofencingService.LocationDetected += (s, a) => { assert = true; };

            _geofencingService.AddLocation(monitoredLocation);
            await _geofencingService.LocationChangedAsync(log);

            assert.Should().BeFalse();
        }

        [Test]
        public async Task LocationChangedAsync_OneMonitoring_TwoLocation_Entered()
        {
            var key = new object();
            var lat = 28.5799448;
            var lng = -16.1348016;
            var log = new LocationLog(lat, lng);
            bool assert = false;

            var monitoredLocation = new MonitoredLocation(key, lat, lng, Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Enter);

            _geofencingService.LocationDetected += (s, a) => { assert = true; };

            _geofencingService.AddLocation(monitoredLocation);
            await _geofencingService.LocationChangedAsync(log);
            await _geofencingService.LocationChangedAsync(log);

            assert.Should().BeTrue();
        }

        [Test]
        public async Task LocationChangedAsync_OneMonitoring_TwoLocation_OneWrong_NoAction()
        {
            var key = new object();
            var lat = 28.5799448;
            var lng = -16.1348016;
            var log = new LocationLog(lat, lng);
            bool assert = false;

            var monitoredLocation = new MonitoredLocation(key, lat, lng, Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Enter);

            _geofencingService.LocationDetected += (s, a) => { assert = true; };

            _geofencingService.AddLocation(monitoredLocation);
            await _geofencingService.LocationChangedAsync(log);
            await _geofencingService.LocationChangedAsync(new LocationLog(51.123213, 17.56565));

            assert.Should().BeFalse();
        }

        [Test]
        public async Task LocationChangedAsync_OneMonitoring_TwoLocation_NoExit()
        {
            var key = new object();
            var lat = 28.5799448;
            var lng = -16.1348016;
            var log = new LocationLog(lat, lng);
            bool assert = false;

            var monitoredLocation = new MonitoredLocation(key, lat, lng, Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Exit);

            _geofencingService.LocationDetected += (s, a) => { assert = true; };

            _geofencingService.AddLocation(monitoredLocation);
            await _geofencingService.LocationChangedAsync(log);
            await _geofencingService.LocationChangedAsync(new LocationLog(51.123213, 17.56565));

            assert.Should().BeFalse();
        }

        [Test]
        public async Task LocationChangedAsync_OneMonitoring_ThreeLocation_Exit()
        {
            var key = new object();
            var lat = 28.5799448;
            var lng = -16.1348016;
            var log = new LocationLog(51.515, 17.51251);
            bool assert = false;

            var monitoredLocation = new MonitoredLocation(key, lat, lng, Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Exit);

            _geofencingService.LocationDetected += (s, a) => { assert = true; };

            _geofencingService.AddLocation(monitoredLocation);
            await _geofencingService.LocationChangedAsync(log);
            await _geofencingService.LocationChangedAsync(log);
            await _geofencingService.LocationChangedAsync(log);

            assert.Should().BeTrue();
        }

        [Test]
        public async Task LocationChangedAsync_OneMonitoring_ThreeLocation_ExitAndRemove()
        {
            var key = new object();
            var lat = 28.5799448;
            var lng = -16.1348016;
            var log = new LocationLog(51.515, 17.51251);
            bool assert = false;

            var monitoredLocation = new MonitoredLocation(key, lat, lng, Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Exit);

            _geofencingService.LocationDetected += (s, a) => { assert = true; };

            _geofencingService.AddLocation(monitoredLocation);
            await _geofencingService.LocationChangedAsync(log);
            await _geofencingService.LocationChangedAsync(log);
            await _geofencingService.LocationChangedAsync(log);


            assert.Should().BeTrue();
        }

        [Test]
        public async Task LocationChanged_SubscriberNotified()
        {
            var key = new object();
            var lat = 28.5799448;
            var lng = -16.1348016;
            var log = new LocationLog(51.515, 17.51251);
            bool assert = false;

            var monitoredLocation = new MonitoredLocation(key, lat, lng, Distance.FromMeters(50), Distance.FromMeters(150), GeofenceDirection.Exit);

            _geofencingService.LocationChanged += (s, a) => { assert = true; };

            await _geofencingService.LocationChangedAsync(log);

            assert.Should().BeTrue();
        }
    }
}