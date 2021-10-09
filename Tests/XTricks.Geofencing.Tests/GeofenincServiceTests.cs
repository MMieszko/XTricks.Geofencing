using FluentAssertions;
using NUnit.Framework;
using System.Linq;
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

    }
}