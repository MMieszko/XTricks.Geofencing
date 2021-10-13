using System;
using XTricks.Shared.Contracts;
using XTricks.Shared.Models;

namespace XTricks.Geofencing
{
    public class MonitoredLocation
    {
        /// <summary>
        /// Unique identifier of tracked location
        /// </summary>
        public object Key { get; }
        /// <summary>
        /// Tracked location
        /// </summary>
        public ILocation Location { get; }
        /// <summary>
        /// Radius area of tracked location to fire enter event.
        /// The value cannot be be higher or equal of <see cref="RadiusExit"/>
        /// </summary>
        public Distance RadiusEnter { get; }
        /// <summary>
        /// Radius area of tracked location to fire exit event.
        /// The value cannot be be less or equal of <see cref="RadiusExit"/>
        /// </summary>
        public Distance RadiusExit { get; }
        /// <summary>
        /// Expectation of which event to fire for given location. 
        /// The <see cref="GeofenceDirection"/> is an flag enum. 
        /// It is possible to track and enter events at once.
        /// </summary>
        public GeofenceDirection Expectation { get; }
        /// <summary>
        /// Determine if location should be kept after exit or enter event appears
        /// </summary>
        public bool RemoveAfterDetected { get; }
        public virtual ILocationDetector Detector => new LocationDetector(this);

        public MonitoredLocation(object key, double latitude, double longitude, Distance radiusEnter, Distance radiusExit, GeofenceDirection expectation, bool remove)
        {
            if (radiusEnter >= radiusExit)
                throw new ArgumentException("Enter radius cannot be higher or equal to radius exit");

            if (key == null)
                throw new ArgumentException("Key of location cannot be null");

            this.Key = key;
            this.Location = new LocationLog(latitude, longitude);
            this.RadiusEnter = radiusEnter;
            this.RadiusExit = radiusExit;
            this.Expectation = expectation;
            this.RemoveAfterDetected = remove;
        }

        public MonitoredLocation(object key, double latitude, double longitude, Distance radiusEnter, Distance radiusExit, GeofenceDirection expectation)
                 : this(key, latitude, longitude, radiusEnter, radiusExit, expectation, false)
        {

        }

        public MonitoredLocation(object key, ILocation location, Distance radiusEnter, Distance radiusExit, GeofenceDirection expectation, bool remove)
                : this(key, location.Latitude, location.Longitude, radiusEnter, radiusExit, expectation, remove)
        {

        }

        public MonitoredLocation(object key, ILocation location, Distance radiusEnter, Distance radiusExit, GeofenceDirection expectation)
            : this(key, location, radiusEnter, radiusExit, expectation, false)
        {

        }
    }
}
