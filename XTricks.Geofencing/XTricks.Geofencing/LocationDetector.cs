using System.Collections.Generic;
using System.Linq;
using XTricks.Shared.Contracts;
using XTricks.Shared.Extensions;

namespace XTricks.Geofencing
{
    public class LocationDetector : ILocationDetector
    {
        public GeofenceDirection LocationExpectation => Monitored.Expectation;
        public MonitoredLocation Monitored { get; }

        public LocationDetector(MonitoredLocation monitored)
        {
            this.Monitored = monitored;
        }

        public GeofenceDirection Detect(List<ILocation> coordinates)
        {
            if (this.LocationExpectation.HasFlag(GeofenceDirection.Enter))
            {
                if (this.IsIn(coordinates))
                    return GeofenceDirection.Enter;
            }
            if (this.LocationExpectation.HasFlag(GeofenceDirection.Exit))
            {
                if (IsOut(coordinates))
                    return GeofenceDirection.Exit;
            }

            return GeofenceDirection.None;
        }

        protected virtual bool IsIn(IEnumerable<ILocation> candidates)
        {
            var result = candidates.Take(2).All(x => x.DistanceTo(Monitored.Location).Meters <= Monitored.RadiusEnter.Meters);

            return result;
        }

        protected virtual bool IsOut(IEnumerable<ILocation> candidates)
        {
            var result = candidates.Take(3).All(x => x.DistanceTo(Monitored.Location).Meters >= Monitored.RadiusExit.Meters);

            return result;
        }
    }
}
