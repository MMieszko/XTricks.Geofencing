using System.Collections.Generic;
using XTricks.Shared.Contracts;

namespace XTricks.Geofencing.Abstractions
{
    public interface ILocationDetector
    {
        GeofenceDirection Detect(List<ILocation> coordinates);
    }
}
