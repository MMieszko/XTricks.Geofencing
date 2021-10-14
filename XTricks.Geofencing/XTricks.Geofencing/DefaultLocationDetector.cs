using System.Collections.Generic;
using System.Linq;
using XTricks.Shared.Contracts;
using XTricks.Shared.Extensions;

namespace XTricks.Geofencing
{
    public class DefaultLocationDetector : ILocationDetector
    {
        public GeofenceDirection LocationExpectation => Monitored.Expectation;
        public MonitoredLocation Monitored { get; }

        public DefaultLocationDetector(MonitoredLocation monitored)
        {
            this.Monitored = monitored;
        }

        public virtual GeofenceDirection Detect(List<ILocation> coordinates)
        {
            if (coordinates.Count < 2)
                return GeofenceDirection.None;

            var reversedCoordinates = Enumerable.Reverse(coordinates).ToList();

            if (this.LocationExpectation.HasFlag(GeofenceDirection.Enter))
            {
                if (this.IsIn(reversedCoordinates))
                    return GeofenceDirection.Enter;
            }
            if (this.LocationExpectation.HasFlag(GeofenceDirection.Exit))
            {
                if (IsOut(reversedCoordinates))
                    return GeofenceDirection.Exit;
            }

            return GeofenceDirection.None;
        }

        private bool IsIn(IEnumerable<ILocation> candidates)
        {
            var result = candidates.Take(2).All(x => x.DistanceTo(Monitored.Location).Meters <= Monitored.RadiusEnter.Meters);

            return result;
        }

        private bool IsOut(IEnumerable<ILocation> candidates)
        {
            var result = candidates.Take(3).All(x => x.DistanceTo(Monitored.Location).Meters >= Monitored.RadiusExit.Meters);

            return result;
        }
    }
}
