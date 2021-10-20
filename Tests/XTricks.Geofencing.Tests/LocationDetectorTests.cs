using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using XTricks.Geofencing.Abstractions;
using XTricks.Shared.Contracts;
using XTricks.Shared.Models;

namespace XTricks.Geofencing.Tests
{
    [TestFixture]
    public class LocationDetectorTests
    {
        [Test]
        public void NoLocation_None()
        {
            var location = new MonitoredLocation(this, 1, 1, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Enter);

            location.Detector.Detect(Enumerable.Empty<ILocation>().ToList()).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void LastOneIn_ShouldNotEnter()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Enter);

            var locations = new List<ILocation> { otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, monitoredCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void LastTwoIn_ShouldEnter()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Enter);

            var locations = new List<ILocation> { otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, monitoredCoords, monitoredCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.Enter);
        }

        [Test]
        public void LastTwoNotIn_PreviousIn_ShouldNotEnter()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Enter);

            var locations = new List<ILocation> { otherCoords, otherCoords, monitoredCoords, monitoredCoords, otherCoords, otherCoords, otherCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void LastOneNotIn_AllOtherIn_ShouldNotEnter()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Enter);

            var locations = new List<ILocation> { monitoredCoords, monitoredCoords, monitoredCoords, monitoredCoords, monitoredCoords, otherCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void LastOneOut_ShouldNotExit()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Exit);

            var locations = new List<ILocation> { otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, monitoredCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void LastThreeOut_ShouldExit()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Exit);

            var locations = new List<ILocation> { otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, monitoredCoords, monitoredCoords, otherCoords, otherCoords, otherCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.Exit);
        }

        [Test]
        public void LastTwoNotOut_PreviousOut_ShouldNotExit()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Exit);

            var locations = new List<ILocation> { otherCoords, monitoredCoords, monitoredCoords, monitoredCoords, monitoredCoords, otherCoords, otherCoords, monitoredCoords, otherCoords, otherCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void LastOneOut_AllOtherOut_ShouldNotExit()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Exit);

            var locations = new List<ILocation> { monitoredCoords, monitoredCoords, monitoredCoords, monitoredCoords, monitoredCoords, otherCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void ExpectingEnter_LastThreeOut_ShouldNotOut()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Enter);

            var locations = new List<ILocation> { otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, monitoredCoords, monitoredCoords, otherCoords, otherCoords, otherCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void ExpectingExit_LastTwoIn_ShouldNotEnter()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Exit);

            var locations = new List<ILocation> { otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, monitoredCoords, monitoredCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.None);
        }

        [Test]
        public void ExpectingBoth_LastThreeOut_ShouldOut()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Enter | GeofenceDirection.Exit);

            var locations = new List<ILocation> { otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, monitoredCoords, monitoredCoords, otherCoords, otherCoords, otherCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.Exit);
        }

        [Test]
        public void ExpectingExit_LastTwoIn_ShouldEnter()
        {
            var otherCoords = new LocationLog(51.5353, 17.4545);
            var monitoredCoords = new LocationLog(28.579961, -16.1346);


            var location = new MonitoredLocation(this, monitoredCoords, Distance.FromMeters(5), Distance.FromMeters(6), GeofenceDirection.Enter | GeofenceDirection.Exit);

            var locations = new List<ILocation> { otherCoords, otherCoords, otherCoords, otherCoords, otherCoords, monitoredCoords, monitoredCoords };


            location.Detector.Detect(locations).Should().Be(GeofenceDirection.Enter);
        }
    }
}