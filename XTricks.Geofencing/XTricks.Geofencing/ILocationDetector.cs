using System.Collections.Generic;
using XTricks.Shared.Contracts;

namespace XTricks.Geofencing
{
    public interface ILocationDetector
    {
        GeofenceDirection Detect(List<ILocation> previousLocations);
    }
}
