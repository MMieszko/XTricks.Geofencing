using System;

namespace XTricks.Geofencing.Abstractions
{
    public class LocationDetectedEventArgs : EventArgs
    {
        public MonitoredLocation Location { get; }
        public DateTime DateTime { get; }
        public GeofenceDirection Direction { get; }

        public LocationDetectedEventArgs(MonitoredLocation location, GeofenceDirection direction, DateTime dateTime)
        {
            this.Location = location;
            this.Direction = direction;
            this.DateTime = dateTime;
        }

        public LocationDetectedEventArgs(MonitoredLocation location, GeofenceDirection direction)
            : this(location, direction, DateTime.Now)
        {

        }
    }
}
