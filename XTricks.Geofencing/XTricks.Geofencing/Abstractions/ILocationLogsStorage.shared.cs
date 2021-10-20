using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTricks.Geofencing.Abstractions
{
    public interface ILocationLogsStorage
    {
        Task AddAsync(LocationLog location);
        Task<List<LocationLog>> GetAsync();
    }
}
